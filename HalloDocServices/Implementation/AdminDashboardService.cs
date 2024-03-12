using HalloDocEntities.Models;
using HalloDocRepository.Interface;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using HalloDocServices.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO.Compression;
using System.Net.Mail;
using System.Net;


namespace HalloDocServices.Implementation
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly INotesAndLogsRepository _notesAndLogsRepository;
        private readonly ICommonRepository _commonRepository;

        public AdminDashboardService(IUserRepository userRepository, IRequestRepository requestRepository, IPhysicianRepository physicianRepository, INotesAndLogsRepository notesAndLogsRepository, ICommonRepository commonRepository)
        {
            _userRepository = userRepository;
            _requestRepository = requestRepository;
            _physicianRepository = physicianRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
            _commonRepository = commonRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
         }

        public async Task<AdminDashboardViewModel> GetViewModelData(int requestStatus)
        {
            AdminDashboardViewModel viewModel = new AdminDashboardViewModel();
            viewModel.RequestStatus = requestStatus;
            viewModel.NewRequestCount = await _requestRepository.GetNewRequestCount();
            viewModel.PendingRequestCount = await _requestRepository.GetPendingRequestCount();
            viewModel.ActiveRequestCount = await _requestRepository.GetActiveRequestCount();
            viewModel.ConcludeRequestCount = await _requestRepository.GetConcludeRequestCount();
            viewModel.ToCloseRequestCount = await _requestRepository.GetToCloseRequestCount();
            viewModel.UnpaidRequestCount = await _requestRepository.GetUnpaidRequestCount();
            return viewModel;
        }

        #region Encode_Decode
        public string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }
        public string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }
        #endregion

        public List<RequestRowViewModel> GetViewModelData(int requestStatus, int? requestType, string? searchPattern, int? searchRegion)
        {
            List<RequestRowViewModel> requestRows = new List<RequestRowViewModel>();
            var requests = _requestRepository.GetAllIEnumerableRequests().AsQueryable();

            

            int[] myarray = new int[3];
            switch (requestStatus)
            {
                case 1:
                    myarray[0] = 1;
                    break;

                case 2:
                    myarray[0] = 2;
                    break;

                case 3:
                    myarray[0] = 4;
                    myarray[1] = 5;
                    break;

                case 4:
                    myarray[0] = 6;
                    break;

                case 5:
                    myarray[0] = 3;
                    myarray[1] = 7;
                    myarray[2] = 8;
                    break;

                case 6:
                    myarray[0] = 9;
                    break;
            }

            requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Include(x => x.RequestStatusLogs).Where(x => myarray.Contains(x.Status));

            /*switch(requestStatus)
            {
                case 1: 
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 1);
                    break;

                case 2:
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 2);
                    break;

                case 3:
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 4 || x.Status == 5);
                    break;

                case 4:
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 6);
                    break;

                case 5:
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 3 || x.Status == 7 || x.Status == 8);
                    break;

                case 6:
                    requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Where(x => x.Status == 9);
                    break;
            }*/

            if (requestType != null)
            {
                requests = requests.Where(x => x.RequestTypeId == requestType);
            }

            

            if(searchPattern != null)
            {
                /*x.RequestClients.FirstOrDefault() != null ? (x.RequestClients.FirstOrDefault().FirstName != null ? x.RequestClients.FirstOrDefault().FirstName : "") : ""*/
                //requests = requests.Where(x => x.FirstName.Contains(searchPattern));
                requests = requests.Where(x => EF.Functions.Like(x.RequestClients.FirstOrDefault().FirstName, "%" + searchPattern + "%"));
            }

            if(searchRegion != null)
            {
                requests = requests.Where(x => x.RequestClients.FirstOrDefault().RegionId == searchRegion);
            }

            foreach(var request in requests)
            {
                RequestClient? requestClient = request.RequestClients.FirstOrDefault();
                int date = requestClient?.IntDate??0;
                int year = requestClient?.IntYear ?? 0;
                string month = requestClient?.StrMonth ?? "";

                List<string> notes = new List<string>();
                foreach(var log in request.RequestStatusLogs)
                {
                    if(log.Notes != null)
                    {
                        notes.Add(log.Notes);
                    }
                }

                requestRows.Add(new()
                {
                    DashboardRequestStatus = requestStatus,
                    RequestStatus = request.Status,
                    RequestId = request.RequestId,
                    RequestType = request.RequestTypeId,
                    PatientFullName = requestClient?.FirstName + " " + requestClient?.LastName,
                    PatientEmail = requestClient?.Email,
                    DateOfBirth = month + " " + date + "," + year,
                    RequestorName = request.FirstName + " " + request.LastName,
                    PhysicianName = request.Physician?.FirstName + " " + request.Physician?.LastName,
                    //DateOfService
                    RequestedDate = request.CreatedDate.ToLongDateString(),
                    PatientPhoneNumber = requestClient?.PhoneNumber,
                    SecondPhoneNumber = request.PhoneNumber,
                    Address = requestClient?.Address,
                    Region = requestClient?.RegionId,
                    //Notes
                    Notes = notes,

                }) ;
            }

            return requestRows;
        }

        public ViewCaseViewModel GetViewCaseViewModelData(ViewCaseViewModel CaseInfo)
        {
             

            var requestFetched = _requestRepository.GetIQueryableRequestByRequestId(CaseInfo.RequestId);

            var request = requestFetched.Include(x => x.RequestClients).Include(x => x.RequestBusinesses).Include(x => x.RequestBusinesses).ThenInclude(x => x.Business).FirstOrDefault();

            if(request  != null)
            {
                var requestClient = request?.RequestClients.FirstOrDefault();
                var businessName = request?.RequestBusinesses.FirstOrDefault()?.Business.Name;

                CaseInfo.RequestId = request?.RequestId ?? 0;
                CaseInfo.Status = request?.Status;
                CaseInfo.RequestType = request?.RequestTypeId;
                CaseInfo.ConfirmationNumber = request?.ConfirmationNumber;

                if (requestClient != null)
                {
                    CaseInfo.Symptoms = requestClient?.Notes;
                    CaseInfo.FirstName = requestClient?.FirstName;
                    CaseInfo.LastName = requestClient?.LastName;

                    if (requestClient?.IntDate.HasValue ?? false && requestClient.IntYear.HasValue && requestClient.StrMonth != null)
                    {
                        DateTime monthDateTime = DateTime.ParseExact(requestClient.StrMonth??"", "MMMM", CultureInfo.InvariantCulture);
                        int month = monthDateTime.Month;
                        DateOnly date = new DateOnly((int)(requestClient.IntYear ?? 0), month, requestClient.IntDate.Value);
                        CaseInfo.DOB = date.ToString("yyyy-MM-dd");
                    }
                    CaseInfo.Email = requestClient?.Email;
                    CaseInfo.PhoneNumber = requestClient?.PhoneNumber;
                    CaseInfo.Region = requestClient?.State;
                    if (businessName != null)
                    {
                        CaseInfo.BusinessNameOrAddress = businessName;
                    }
                    else
                    {
                        CaseInfo.BusinessNameOrAddress = requestClient?.Address;
                    }
                    CaseInfo.Room = requestClient?.Location;
                }
            }

            return CaseInfo;
        }

        public async Task<bool> UpdateViewCaseInfo(ViewCaseViewModel CaseInfo)
        {
            var requestClient = await _requestRepository.GetRequestClientByRequestId(CaseInfo.RequestId);
            if(requestClient == null) 
            {
                return false;
            }
            else
            {
                requestClient.Email = CaseInfo.Email;
                requestClient.PhoneNumber = CaseInfo.PhoneNumber;
                requestClient.NotiMobile = CaseInfo.PhoneNumber;

                await _requestRepository.UpdateRequestClient(requestClient);

                return true;
            }
        }

        public async Task<ViewNotesViewModel> GetViewNotesViewModelData(int requestId)
        {
            ViewNotesViewModel ViewNotes = new ViewNotesViewModel();

            var requestNotes = await _notesAndLogsRepository.GetNoteByRequestId(requestId);
            var requestStatusLogs = _notesAndLogsRepository.GetStatusLogsByRequestId(requestId).ToList();

            ViewNotes.RequestId = requestId;
            ViewNotes.AdminNotes = requestNotes?.AdminNotes;
            ViewNotes.PhysicianNotes = requestNotes?.PhysicianNotes;
            List<string> transfernotes = new List<string>();
            foreach(var log in requestStatusLogs)
            {
                if(log.Notes != null)
                {
                    if (log.Status == 3)
                    {
                        int index = log.Notes.LastIndexOf("-");
                        if (log.PhysicianId != null)
                        {
                            ViewNotes.PhysicianCancellationNotes = log.Notes.Substring(index + 2);
                        }
                        else if (log.AdminId != null)
                        {
                            //ViewNotes.AdminCancellationNotes = log.Notes;
                            ViewNotes.AdminCancellationNotes = log.Notes.Substring(index + 2);
                        }
                    }
                    else if (log.Status == 7)
                    {
                        int index = log.Notes.LastIndexOf("-");
                        ViewNotes.PatientCancellationNotes = log.Notes.Substring(index + 2);
                    }
                    
                    transfernotes.Add(log.Notes);
                    
                }
            }
            ViewNotes.TransferNotes = transfernotes;

            return ViewNotes;
        }

        public async Task<bool> AddAdminNote(int requestId, string AdminNotesInput)
        {
            var requestNote = await _notesAndLogsRepository.GetNoteByRequestId(requestId);
            if (requestNote == null)
            {
                RequestNote note = new RequestNote();

                note.AdminNotes = AdminNotesInput;
                note.RequestId = requestId;
                note.CreatedBy = "sd";
                note.CreatedDate = DateTime.Now;

                await _notesAndLogsRepository.AddRequestNote(note);

                return true;
            }
            else
            {
                requestNote.AdminNotes = AdminNotesInput;

                await _notesAndLogsRepository.UpdateRequestNote(requestNote);

                return true;
            }
        }

        public CancelCaseViewModel GetCancelCaseViewModelData(CancelCaseViewModel CancelCase)
        {
            CancelCase.CaseTags = new List<string>();
            CancelCase.CaseTagIds = new List<int>();

            var requestFetched = _requestRepository.GetIQueryableRequestByRequestId(CancelCase.RequestId);
            var request = requestFetched.Include(x => x.RequestClients).ToList();

            var CaseTags = _commonRepository.GetAllCaseTags();

            foreach(var tag in CaseTags)
            {
                if(tag == null) continue;
                if(tag.Name != null)
                {
                    CancelCase.CaseTags.Add(tag.Name);
                }
                CancelCase.CaseTagIds.Add(tag.CaseTagId);
            }

            RequestClient? requestClient = request[0].RequestClients?.FirstOrDefault();
            CancelCase.PatientFullName = requestClient?.FirstName + " " + requestClient?.LastName;

            return CancelCase;
        }

        public async Task<bool> CancelCase(CancelCaseViewModel CancelCase)
        {
            var requestFetched = await _requestRepository.GetRequestByRequestId(CancelCase.RequestId);
            if(requestFetched != null)
            {
                requestFetched.Status = 3;
                requestFetched.CaseTag = CancelCase.CaseTagId.ToString();

                await _requestRepository.UpdateRequest(requestFetched);

                RequestStatusLog log = new RequestStatusLog();
                log.RequestId = CancelCase.RequestId;
                log.Status = requestFetched.Status;
                log.AdminId = CancelCase.AdminId??1;
                log.Notes = "Admin cancelled the case on " + DateOnly.FromDateTime(DateTime.Now) + " at " + DateTime.Now.ToLongTimeString() + " - " + CancelCase.AdminCancellationNote;
                log.CreatedDate = DateTime.Now;

                await _notesAndLogsRepository.AddRequestStatusLog(log);
            }
            
            return true;
        }

        public AssignCaseViewModel GetAssignCaseViewModelData(AssignCaseViewModel AssignCase)
        {
            var regions = _commonRepository.GetAllRegions();
            var physicians = _physicianRepository.GetAllPhysicians();

            if(AssignCase.RegionId != 0)
            {
                physicians = physicians.Where(x => x.RegionId == AssignCase.RegionId).ToList();
            }

            Dictionary<int, string> RegionList = new Dictionary<int, string>();
            Dictionary<int, string> PhysicianList = new Dictionary<int, string>();

            foreach (var region in regions)
            {
                RegionList.Add(region.RegionId, region.Name);
            }

            foreach(var physician in physicians)
            {
                PhysicianList.Add(physician.PhysicianId, physician.FirstName +  " " + physician.LastName);
            }

            AssignCase.RegionList = RegionList;
            AssignCase.PhysicianList = PhysicianList;

            return AssignCase;
        }

        public async Task<bool> AssignCase(AssignCaseViewModel AssignCase)
        {
            var requestFetched = await _requestRepository.GetRequestByRequestId(AssignCase.RequestId);
            var physicianFetched = _physicianRepository.GetPhysicianByPhysicianId(AssignCase.PhysicianId ?? 0);

            requestFetched.PhysicianId = AssignCase.PhysicianId;
            requestFetched.Status = 2;

            await _requestRepository.UpdateRequest(requestFetched);

            RequestStatusLog log = new RequestStatusLog();
            log.RequestId = AssignCase.RequestId;
            log.Status = requestFetched.Status;
            log.AdminId = AssignCase.AdminId ?? 1;
            log.Notes = "Admin assigned the case to DR. " + physicianFetched.FirstName + " " + physicianFetched.LastName + "on " + DateOnly.FromDateTime(DateTime.Now) + " at " + DateTime.Now.ToLongTimeString() + " - " + AssignCase.Description;
            log.CreatedDate = DateTime.Now;

            await _notesAndLogsRepository.AddRequestStatusLog(log);

            return true;
        }

        public async Task<bool> TransferRequest(AssignCaseViewModel TransferRequest)
        {
            var requestFetched = await _requestRepository.GetRequestByRequestId(TransferRequest.RequestId);
            var physicianFetched = _physicianRepository.GetPhysicianByPhysicianId(TransferRequest.PhysicianId ?? 0);

            requestFetched.PhysicianId = TransferRequest.PhysicianId;
            requestFetched.Status = 2;

            await _requestRepository.UpdateRequest(requestFetched);

            RequestStatusLog log = new RequestStatusLog();
            log.RequestId = TransferRequest.RequestId;
            log.Status = requestFetched.Status;
            log.AdminId = TransferRequest.AdminId ?? 1;
            log.Notes = "Admin transferred the case to DR. " + physicianFetched.FirstName + " " + physicianFetched.LastName + "on " + DateOnly.FromDateTime(DateTime.Now) + " at " + DateTime.Now.ToLongTimeString() + " - " + TransferRequest.Description;
            log.CreatedDate = DateTime.Now;

            await _notesAndLogsRepository.AddRequestStatusLog(log);

            return true;
        }

        public async Task<BlockRequestViewModel> GetBlockRequestViewModelData(BlockRequestViewModel BlockRequest)
        {
            var requestClient = await _requestRepository.GetRequestClientByRequestId(BlockRequest.RequestId ?? 0);
            if(requestClient != null)
            {
                BlockRequest.PatientFullName = requestClient.FirstName + " " + requestClient.LastName;
            }

            return BlockRequest;
        }

        public async Task<bool> BlockRequest(BlockRequestViewModel BlockRequest)
        {
            var requestFetched = await _requestRepository.GetRequestByRequestId(BlockRequest.RequestId ?? 0);
            if (requestFetched == null)
            {
                return false;
            }
            requestFetched.Status = 11;
            await _requestRepository.UpdateRequest(requestFetched);

            var requestClientFetched = await _requestRepository.GetRequestClientByRequestId(requestFetched.RequestId);

            BlockRequest blockRequest = new BlockRequest();
            if(requestClientFetched != null)
            {
                blockRequest.PhoneNumber = requestClientFetched.PhoneNumber;
                blockRequest.Email = requestClientFetched.Email;
            }
            blockRequest.IsActive = true;
            blockRequest.Reason = BlockRequest.Description;
            blockRequest.RequestId = requestFetched.RequestId;
            blockRequest.CreatedDate = DateTime.Now;

            await _requestRepository.CreateBlockRequest(blockRequest);

            RequestStatusLog log = new RequestStatusLog();
            log.RequestId = requestFetched.RequestId;
            log.Status = requestFetched.Status;
            log.AdminId = BlockRequest.AdminId ?? 1;
            log.Notes = "Admin blocked the case on "  + DateOnly.FromDateTime(DateTime.Now) + " at " + DateTime.Now.ToLongTimeString() + " - " + BlockRequest.Description;
            log.CreatedDate = DateTime.Now;

            await _notesAndLogsRepository.AddRequestStatusLog(log);

            return true;


        }

        public ViewDocumentsViewModel GetViewUploadsViewModelData(ViewDocumentsViewModel ViewUploads)
        {
            var requestFetched = _requestRepository.GetIQueryableRequestByRequestId(ViewUploads.RequestId);

            requestFetched = requestFetched.Include(x => x.RequestWiseFiles).Include(x => x.RequestClients);

            var data = requestFetched.FirstOrDefault()?.RequestWiseFiles;
            var requestwisefiles = data?.AsQueryable().Include(x => x.Admin).Include(x => x.Physician).Where(x => x.IsDeleted == false || x.IsDeleted == null).ToList();

            var requestClient = requestFetched.FirstOrDefault()?.RequestClients.FirstOrDefault();

            var request = requestFetched.FirstOrDefault();

            ViewUploads.RequestClientId = requestClient?.RequestClientId;
            ViewUploads.PatientFullName = requestClient?.FirstName + " " + requestClient?.LastName;

            List<RequestFileViewModel> requestfilelist = new List<RequestFileViewModel>();
            if(requestwisefiles != null)
            {
                foreach (var file in requestwisefiles)
                {
                    requestfilelist.Add(new RequestFileViewModel
                    {
                        FileId = file.RequestWiseFileId,
                        FileName = Path.GetFileName(file.FileName),
                        Uploader = (file.AdminId != null ? file.Admin?.FirstName : (file.PhysicianId != null ? file.Physician?.FirstName : request?.FirstName)),
                        UploadDate = DateOnly.FromDateTime(file.CreatedDate),
                        FilePath = file.FileName
                    });
                }
            }

            ViewUploads.FileInfo = requestfilelist;

            return ViewUploads;
        }

        public async Task UploadFiles(IEnumerable<IFormFile> MultipleFiles, int requestId)
        {
            List<string> files = new List<string>();
            foreach (var UploadFile in MultipleFiles)
            {
                string FilePath = "wwwroot\\Upload\\" + requestId;
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
            List<RequestWiseFile> requestWiseFiles = new List<RequestWiseFile>();
            foreach (string file in files)
            {
                var reqwisefileNew = new RequestWiseFile();
                reqwisefileNew.RequestId = requestId;
                reqwisefileNew.FileName = file;
                reqwisefileNew.AdminId = 1;
                reqwisefileNew.CreatedDate = DateTime.Now;

                requestWiseFiles.Add(reqwisefileNew);
            }
            await _requestRepository.CreateRequestWiseFiles(requestWiseFiles);
        }
        

        public async Task<DownloadedFile> DownloadFile(int fileId)
        {
            var requestFileData = await _requestRepository.GetRequestWiseFileByFileId(fileId);

            string FilePath = "wwwroot\\" + requestFileData?.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
            var bytes = System.IO.File.ReadAllBytes(path);
            var newfilename = Path.GetFileName(path);

            DownloadedFile dloadFileData = new DownloadedFile() 
            { 
                RequestId = requestFileData?.RequestId ?? 0,
                FileId = fileId,
                Data = bytes,
                Filename = newfilename
            };

            return dloadFileData;
        }

        public byte[] GetFilesAsZip(List<int> fileIds, int requestId)
        {
            //var filesRow = await _patientService.GetRequestFiles(id);
            var files = _requestRepository.GetRequestWiseFilesByFileIds(fileIds);

            MemoryStream ms = new MemoryStream();
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                files.ForEach(file =>
                {
                    string FilePath = "wwwroot\\" + file.FileName;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                    ZipArchiveEntry zipEntry = zip.CreateEntry(Path.GetFileName(file.FileName));
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    using (Stream zipEntryStream = zipEntry.Open())
                    {
                        fs.CopyTo(zipEntryStream);
                    }
                });
            }
                
            return ms.ToArray();
        }

        public async Task<RequestWiseFile> DeleteFile(int fileId)
        {
            var requestFileData = await _requestRepository.GetRequestWiseFileByFileId(fileId);

            requestFileData.IsDeleted = true;

            await _requestRepository.UpdateRequestWiseFile(requestFileData);

            return requestFileData;
        }

        public async Task<bool> DeleteSelectedFiles(List<int> fileIds, int requestId)
        {
            var files = _requestRepository.GetRequestWiseFilesByFileIds(fileIds);

            foreach(var file in files)
            {
                file.IsDeleted = true;
            }

            await _requestRepository.UpdateRequestWiseFiles(files);

            return true;
        }

        public async Task<bool> SendMailWithAttachments(DownloadRequest requestData)
        {
            var files = _requestRepository.GetRequestWiseFilesByFileIds(requestData.SelectedValues);

            var mail = "tatva.dotnet.shreyashdetroja@outlook.com";
            var password = "Dotnet_tatvasoft@14"; 

            var client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            string subject = "Case Files for Request: " + requestData.RequestId + " from HalloDoc@Admin";

            string message = "Find the case files for Request: " + requestData.RequestId + " in the attachments below.";

            string receiver = requestData.EmailValue ?? "";

            string senderDisplayName = "HalloDoc Admin";
            string receiverDisplayName = requestData.EmailValue ?? "";

            MailAddress senderMailAddress = new MailAddress(mail, senderDisplayName);
            MailAddress receiverMailAddress = new MailAddress(receiver, receiverDisplayName);

            using (var mailMessage = new MailMessage(senderMailAddress, receiverMailAddress))
            {
                mailMessage.Subject = subject;
                mailMessage.Body = message;

                // Validate file path and existence
                foreach(var file in files)
                {
                    string FilePath = "wwwroot\\" + file.FileName;
                    string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

                    if (!System.IO.File.Exists(path))
                    {
                        throw new ArgumentException("Invalid file path: " + path);
                    }

                    // Attach the file
                    var attachment = new Attachment(path);
                    mailMessage.Attachments.Add(attachment);
                }

                await client.SendMailAsync(mailMessage);
            }

            return true;
        }

        public string GetProfessionListOptions()
        {
            var healthProfessionTypes = _commonRepository.GetAllHealthProfessionTypes();

            string options = "<option value=\"0\" selected>Select Profession</option>";

            foreach (var profession in healthProfessionTypes)
            {
                string option = $"<option value=\"{profession.HealthProfessionId}\">{profession.ProfessionName}</option>";
                options = options + option;
            }

            return options;
        }

        public string GetVendorListOptions(int professionId)
        {
            var healthProfessionals = _commonRepository.GetAllHealthProfessionals();
            healthProfessionals = healthProfessionals.Where(x => x.ProfessionId == professionId).ToList();

            string options = "<option value=\"0\">Select Business</option>";

            foreach (var professional in healthProfessionals)
            {
                string option = $"<option value=\"{professional.VendorId}\">{professional.VendorName}</option>";
                options = options + option;
            }

            return options;
        }

        public OrdersViewModel GetVendorDetails(int vendorId)
        {
            var vendor = _commonRepository.GetVendorById(vendorId);
            OrdersViewModel VendorDetails = new OrdersViewModel();
            
            VendorDetails.VendorId = vendor.VendorId;
            VendorDetails.BusinessContact = vendor.BusinessContact;
            VendorDetails.Email = vendor.Email;
            VendorDetails.FaxNumber = vendor.FaxNumber;

            return VendorDetails;

        }

        public async Task<bool> SendOrder(OrdersViewModel Order)
        {
            OrderDetail order = new OrderDetail()
            {
                VendorId = Order.VendorId,
                Email = Order.Email,
                RequestId = Order.RequestId,
                FaxNumber = Order.FaxNumber,
                BusinessContact = Order.BusinessContact,
                Prescription = Order.OrderDetails,
                NoOfRefill = Order.NumberOfRefills,
                CreatedBy = "74502e9b-b4b3-49ab-b883-d00dbfd57ad2",
                CreatedDate = DateTime.Now
            };

            await _commonRepository.CreateOrder(order);

            return true;
        }

        public async Task<bool> ClearCase(ClearCaseViewModel ClearCase)
        {
            var requestFetched = await _requestRepository.GetRequestByRequestId(ClearCase.RequestId);
            if (requestFetched != null)
            {
                requestFetched.Status = 10;

                await _requestRepository.UpdateRequest(requestFetched);

                RequestStatusLog log = new RequestStatusLog();
                log.RequestId = requestFetched.RequestId;
                log.Status = requestFetched.Status;
                log.AdminId = ClearCase.AdminId ?? 1;
                log.Notes = "Admin cleared the case on " + DateOnly.FromDateTime(DateTime.Now) + " at " + DateTime.Now.ToLongTimeString();
                log.CreatedDate = DateTime.Now;

                await _notesAndLogsRepository.AddRequestStatusLog(log);
            }

            return true;
        }

        public async Task<bool> SendAgreementViaMail(SendAgreementViewModel SendAgreementInfo)
        {

            var mail = "tatva.dotnet.shreyashdetroja@outlook.com";
            var password = "Dotnet_tatvasoft@14";

            var client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mail, password)
            };

            string subject = "Service Agreement from HalloDoc@Admin";

            string EncryptedRequestId = Encode(SendAgreementInfo.RequestId.ToString());
            string url = "http://localhost:5059/Patient/Agreement?requestId=" + EncryptedRequestId;
            string message = "Click on the link to view the service agreement: " + url ;

            string receiver = SendAgreementInfo.Email ?? "";

            string senderDisplayName = "HalloDoc Admin";
            string receiverDisplayName = "";

            MailAddress senderMailAddress = new MailAddress(mail, senderDisplayName);
            MailAddress receiverMailAddress = new MailAddress(receiver, receiverDisplayName);

            using (var mailMessage = new MailMessage(senderMailAddress, receiverMailAddress))
            {
                mailMessage.Subject = subject;
                mailMessage.Body = message;

                await client.SendMailAsync(mailMessage);
            }

            var requestFetched = await _requestRepository.GetRequestByRequestId(SendAgreementInfo.RequestId);
            if(requestFetched != null)
            {
                requestFetched.IsAgreementSent = true;
                await _requestRepository.UpdateRequest(requestFetched);
            }

            return true;
        }

        public async Task<SendAgreementViewModel> GetSendAgreementViewModelData(SendAgreementViewModel SendAgreementInfo)
        {
            var requestClientFetched = await _requestRepository.GetRequestClientByRequestId(SendAgreementInfo.RequestId);
            var requestFetched = await _requestRepository.GetRequestByRequestId(SendAgreementInfo.RequestId);
            if(requestClientFetched  != null && requestFetched != null)
            {
                SendAgreementInfo.RequestType = requestFetched.RequestTypeId;
                SendAgreementInfo.Email = requestClientFetched.Email;
                SendAgreementInfo.PhoneNumber = requestClientFetched.PhoneNumber;
                SendAgreementInfo.IsAgreementSent = requestFetched.IsAgreementSent;
            }

            return SendAgreementInfo;
        }

        public async Task<bool> CloseCase(int requestId, int adminId)
        {
            var requestFetched = await _requestRepository.GetRequestByRequestId(requestId);
            if(requestFetched != null)
            {
                requestFetched.Status = 9;

                await _requestRepository.UpdateRequest(requestFetched);

                RequestStatusLog log = new RequestStatusLog();
                log.RequestId = requestFetched.RequestId;
                log.Status = requestFetched.Status;
                log.AdminId = adminId;
                log.Notes = "Admin closed the case on " + DateOnly.FromDateTime(DateTime.Now) + " at " + DateTime.Now.ToLongTimeString();
                log.CreatedDate = DateTime.Now;

                await _notesAndLogsRepository.AddRequestStatusLog(log);
            }

            return true;
        }
    }
}
