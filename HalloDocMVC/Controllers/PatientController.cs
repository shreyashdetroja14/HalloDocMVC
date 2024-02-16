using HalloDocMVC.Models;
using HalloDocMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO.Compression;

namespace HalloDocMVC.Controllers
{
    public class PatientController : Controller
    {
        private readonly HallodocContext _context;

        public PatientController(HallodocContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Dashboard(string id)
        {
            /*if (string.IsNullOrWhiteSpace(id))
            {
                
            }*/

            var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == id);

            if (userFetched == null)
            {
                return NotFound();
            }
            var filecountgrouped = (from rwf in _context.RequestWiseFiles
                                    group rwf by rwf.RequestId into gp
                                    select new
                                    {
                                        RequestId = gp.Key,
                                        Cnt = gp.Count()
                                    }).ToList();
                                   

            var data = from requests in _context.Requests.ToList()
                               join count in filecountgrouped
                               on requests.RequestId equals count.RequestId into joined
                               from j in joined.DefaultIfEmpty()
                       where requests.UserId == userFetched?.UserId
                       orderby requests.CreatedDate descending
                               select new
                               {
                                   RequestId = requests.RequestId,
                                   CreatedDate = requests.CreatedDate,
                                   Status = requests.Status,
                                   FileCount = j?.Cnt ?? 0,
                                   PhysicianId = requests.PhysicianId ?? 0,
                               };

            List<DashboardRequestViewModel> requestlist = new List<DashboardRequestViewModel>();
            foreach (var r in data)
            {
                /*Debug.Print(($@"""{r.RequestId}"" ""{r.CreatedDate}"" ""{r.FileCount}"" "));*/
                requestlist.Add(new DashboardRequestViewModel
                {
                    RequestId = r.RequestId,
                    CreateDate = DateOnly.FromDateTime(r.CreatedDate),
                    Status = r.Status,
                    Count = r.FileCount,
                    PhysicianId = r.PhysicianId
                });


            }

            //Pass user id to layout
            ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
            ViewBag.UserId = id;

            return View(requestlist);
        }
        
        public async Task<IActionResult> ViewDocuments(int requestid)
        {
            var requestwisefilesFetched = _context.RequestWiseFiles.Where(x => x.RequestId == requestid).ToList();
            var data = from requests in _context.Requests.ToList()
                       join files in requestwisefilesFetched
                       on requests.RequestId equals files.RequestId
                       select new
                       {
                           requests,
                           files
                       };

            List<RequestFileViewModel> requestfilelist = new List<RequestFileViewModel>();
            foreach(var row in data)
            {
                requestfilelist.Add(new RequestFileViewModel
                {
                    FileId = row.files.RequestWiseFileId,
                    FileName = Path.GetFileName(row.files.FileName),
                    Uploader = row.requests.FirstName + " " + row.requests.LastName,
                    UploadDate = DateOnly.FromDateTime(row.files.CreatedDate),
                    FilePath = row.files.FileName
                });
            }
            ViewDocumentsViewModel vrvm = new()
            {
                RequestId = requestid,
                FileInfo = requestfilelist
            };



            // Pass user id to layout
            var requestFetched = await _context.Requests.FirstOrDefaultAsync(x => x.RequestId == requestid);
            if(requestFetched != null)
            {
                var userFetched = await _context.Users.FirstOrDefaultAsync(x => x.UserId == requestFetched.UserId);
                ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;
                
            }



            return View(vrvm);
        }

        public async Task<IActionResult> UploadRequestFile(IEnumerable<IFormFile>? MultipleFiles, int requestid)
        {
            
            if(MultipleFiles != null)
            {
                List<string> files = new List<string>();
                foreach (var UploadFile in MultipleFiles)
                {
                    string FilePath = "wwwroot\\Upload\\" + requestid;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string newfilename = $"{Path.GetFileNameWithoutExtension(UploadFile.FileName)}-{DateTime.Now.ToString("yyyyMMddhhmmss")}.{Path.GetExtension(UploadFile.FileName).Trim('.')}";

                    string fileNameWithPath = Path.Combine(path, newfilename);
                    files.Add(FilePath.Replace("wwwroot\\Upload\\", "/Upload/") + "/" + newfilename);

                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        UploadFile.CopyTo(stream);
                    }
                }
                foreach (string file in files)
                {
                    var reqwisefileNew = new RequestWiseFile();
                    reqwisefileNew.RequestId = requestid;
                    reqwisefileNew.FileName = file;
                    reqwisefileNew.CreatedDate = DateTime.Now;

                    _context.Add(reqwisefileNew);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("ViewDocuments", new { requestid });
        }

        public IActionResult DownloadFile (int id)
        {
            var requestwisefileFetched = _context.RequestWiseFiles.Find(id);
            string FilePath = "wwwroot\\" + requestwisefileFetched?.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
            //var path = "G:\\test\\hallodoccopy\\HalloDocMVC\\wwwroot\\" + requestwisefileFetched?.FileName;
            var bytes = System.IO.File.ReadAllBytes(path);
            var newfilename = Path.GetFileName(path);

            return File(bytes, "application/octet-stream", newfilename);
        }

        public IActionResult DownloadAllFiles (int id)
        {

            var requestFetched = _context.RequestWiseFiles.Find(id);
            if(requestFetched != null) 
            {
                id = requestFetched.RequestId;
            }
            
            var filesRow = _context.RequestWiseFiles.Where(x => x.RequestId == id).ToList();
            MemoryStream ms = new MemoryStream();
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                filesRow.ForEach(file =>
                {
                    string FilePath = "wwwroot\\" + file.FileName;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                    //string path = "G:\\test\\hallodoccopy\\HalloDocMVC\\wwwroot\\" + file.FileName;


                    ZipArchiveEntry zipEntry = zip.CreateEntry(Path.GetFileName(file.FileName));
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    using (Stream zipEntryStream = zipEntry.Open())
                    {
                        fs.CopyTo(zipEntryStream);
                    }
                });
            return File(ms.ToArray(), "application/zip", "download.zip");
        }



        public IActionResult Profile(string userid) 
        {
            ProfileViewModel profile = new ProfileViewModel();
            return View(profile);
        }
    }
}
