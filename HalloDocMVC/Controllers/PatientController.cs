using HalloDocMVC.Models;
using HalloDocMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var userFetched = await _context.Users.FirstOrDefaultAsync(m => m.AspNetUserId == id);

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
            ViewBag.Fullname = userFetched?.FirstName + " " + userFetched?.LastName;

            return View(requestlist);
        }
    }
}
