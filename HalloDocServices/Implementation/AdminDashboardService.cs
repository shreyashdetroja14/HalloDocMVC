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
using HalloDocServices.Constants;
using System.Data;
using ClosedXML.Excel;


namespace HalloDocServices.Implementation
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly INotesAndLogsRepository _notesAndLogsRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IMailService _mailService;

        public AdminDashboardService(IRequestRepository requestRepository, IPhysicianRepository physicianRepository, INotesAndLogsRepository notesAndLogsRepository, ICommonRepository commonRepository, IVendorRepository vendorRepository, IMailService mailService)
        {
            _requestRepository = requestRepository;
            _physicianRepository = physicianRepository;
            _notesAndLogsRepository = notesAndLogsRepository;
            _commonRepository = commonRepository;
            _vendorRepository = vendorRepository;
            _mailService = mailService;
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

        public PaginatedListViewModel<RequestRowViewModel> GetViewModelData(int requestStatus, int? requestType, string? searchPattern, int? searchRegion, int pageNumber)
        {
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

            requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Include(x => x.RequestStatusLogs).Where(x => x.IsDeleted != true && myarray.Contains(x.Status));


            if (requestType != null)
            {
                requests = requests.Where(x => x.RequestTypeId == requestType);
            }

            if(searchPattern != null)
            {
                requests = requests.Where(x => EF.Functions.Like(x.RequestClients.FirstOrDefault().FirstName, "%" + searchPattern + "%"));
            }

            if(searchRegion != null)
            {
                requests = requests.Where(x => x.RequestClients.FirstOrDefault().RegionId == searchRegion);
            }

            int requestCount = requests.Count();
            int pageSize = 5;
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            PagerViewModel PagerData = new PagerViewModel(requestCount, pageNumber, pageSize);

            int requestToSkip = (pageNumber - 1) * pageSize;

            requests = requests.Skip(requestToSkip).Take(pageSize);

            List<RequestRowViewModel> requestRows = new List<RequestRowViewModel>();

            foreach (var request in requests)
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
                    DateOfService = request.AcceptedDate.ToString(),
                    RequestedDate = request.CreatedDate.ToLongDateString(),
                    PatientPhoneNumber = requestClient?.PhoneNumber,
                    SecondPhoneNumber = request.PhoneNumber,
                    Address = requestClient?.Address,
                    Region = requestClient?.RegionId,
                    Notes = notes,

                }) ;
            }

            PaginatedListViewModel<RequestRowViewModel> PaginatedData = new PaginatedListViewModel<RequestRowViewModel>();
            PaginatedData.PagerData = PagerData;
            PaginatedData.DataRows = requestRows;

            return PaginatedData;
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
            var fileNames = files.Select(x => x.FileName).ToList();

            string subject = "Case Files for Request: " + requestData.RequestId + " from HalloDoc@Admin";

            string body = "Find the case files for Request: " + requestData.RequestId + " in the attachments below.";

            List<string> receivers = new List<string>
                {
                    requestData.EmailValue ?? ""
                };

            bool isMailSent = await _mailService.SendMail(receivers, subject, body, false, fileNames);
            return isMailSent;
        }

        public string GetProfessionListOptions()
        {
            var healthProfessionTypes = _vendorRepository.GetAllHealthProfessionTypes();

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
            var healthProfessionals = _vendorRepository.GetAllHealthProfessionals();
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
            var vendor = _vendorRepository.GetVendorById(vendorId);
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
            Request request = await _requestRepository.GetRequestByRequestId(SendAgreementInfo.RequestId);

            if(request == null) { return false; }

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
            string message = "Click on the link to view the service agreement for Request - " + request.ConfirmationNumber + " on HalloDoc Platform: " + url ;

            //string message = "hello";
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

            request.IsAgreementSent = true;
            await _requestRepository.UpdateRequest(request);

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

        public EncounterFormViewModel GetEncounterFormViewModelData(EncounterFormViewModel EncounterFormDetails)
        {
            var encounterFormFetched = _commonRepository.GetEncounterFormByRequestId(EncounterFormDetails.RequestId);
            if(encounterFormFetched?.RequestId != null)
            {
                EncounterFormDetails.EncounterFormId = encounterFormFetched.EncounterFormId;
                EncounterFormDetails.RequestId = encounterFormFetched.RequestId;
                EncounterFormDetails.FirstName = encounterFormFetched.FirstName;
                EncounterFormDetails.LastName = encounterFormFetched.LastName;
                EncounterFormDetails.Location = encounterFormFetched.Location;

                if (encounterFormFetched.IntDate.HasValue && encounterFormFetched.IntYear.HasValue && encounterFormFetched.StrMonth != null)
                {
                    DateTime monthDateTime = DateTime.ParseExact(encounterFormFetched.StrMonth, "MMMM", CultureInfo.InvariantCulture);
                    int month = monthDateTime.Month;
                    DateOnly date = new DateOnly((int)encounterFormFetched.IntYear, month, encounterFormFetched.IntDate.Value);
                    EncounterFormDetails.DOB = date.ToString("yyyy-MM-dd");
                }

                EncounterFormDetails.ServiceDate = encounterFormFetched.ServiceDate.ToString();
                EncounterFormDetails.PhoneNumber = encounterFormFetched.PhoneNumber;
                EncounterFormDetails.Email = encounterFormFetched.Email ?? "";
                EncounterFormDetails.PresentIllnessHistory = encounterFormFetched.PresentIllnessHistory;
                EncounterFormDetails.MedicalHistory = encounterFormFetched.MedicalHistory;
                EncounterFormDetails.Medications = encounterFormFetched.Medications;
                EncounterFormDetails.Allergies = encounterFormFetched.Allergies;
                EncounterFormDetails.Temperature = encounterFormFetched.Temperature;
                EncounterFormDetails.HeartRate = encounterFormFetched.HeartRate;
                EncounterFormDetails.RespirationRate = encounterFormFetched.RespirationRate;
                EncounterFormDetails.BloodPressureSystolic = encounterFormFetched.BloodPressureSystolic;
                EncounterFormDetails.BloodPressureDiastolic = encounterFormFetched.BloodPressureDiastolic;
                EncounterFormDetails.OxygenLevel = encounterFormFetched.OxygenLevel;
                EncounterFormDetails.Pain = encounterFormFetched.Pain;
                EncounterFormDetails.Heent = encounterFormFetched.Heent;
                EncounterFormDetails.Cardiovascular = encounterFormFetched.Cardiovascular;
                EncounterFormDetails.Chest = encounterFormFetched.Chest;
                EncounterFormDetails.Abdomen = encounterFormFetched.Abdomen;
                EncounterFormDetails.Extremities = encounterFormFetched.Extremities;
                EncounterFormDetails.Skin = encounterFormFetched.Skin;
                EncounterFormDetails.Neuro = encounterFormFetched.Neuro;
                EncounterFormDetails.Other = encounterFormFetched.Other;
                EncounterFormDetails.Diagnosis = encounterFormFetched.Diagnosis;
                EncounterFormDetails.TreatmentPlan = encounterFormFetched.TreatmentPlan;
                EncounterFormDetails.MedicationsDispensed = encounterFormFetched.MedicationsDispensed;
                EncounterFormDetails.Procedures = encounterFormFetched.Procedures;
                EncounterFormDetails.FollowUp = encounterFormFetched.FollowUp;
                EncounterFormDetails.CreatedDate = encounterFormFetched.CreatedDate;
                EncounterFormDetails.ModifiedDate = encounterFormFetched.ModifiedDate;
                EncounterFormDetails.IsFinalized = encounterFormFetched.IsFinalized;
                EncounterFormDetails.FinalizedDate = encounterFormFetched.FinalizedDate;

            }
            return EncounterFormDetails;
        }

        public async Task<bool> UpdateEncounterForm(EncounterFormViewModel EncounterFormDetails)
        {
            var encounterFormToUpdate = _commonRepository.GetEncounterFormByRequestId(EncounterFormDetails.RequestId);

            if (encounterFormToUpdate != null)
            {
                encounterFormToUpdate.FirstName = EncounterFormDetails.FirstName;
                encounterFormToUpdate.LastName = EncounterFormDetails.LastName;
                encounterFormToUpdate.Location = EncounterFormDetails.Location;

                if (EncounterFormDetails.DOB != null)
                {
                    DateTime dateTime = DateTime.ParseExact(EncounterFormDetails.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    encounterFormToUpdate.IntYear = dateTime.Year;
                    encounterFormToUpdate.StrMonth = dateTime.ToString("MMMM");
                    encounterFormToUpdate.IntDate = dateTime.Day;
                }

                if(EncounterFormDetails.ServiceDate != null)
                {
                    encounterFormToUpdate.ServiceDate = DateTime.ParseExact(EncounterFormDetails.ServiceDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                }

                encounterFormToUpdate.PhoneNumber = EncounterFormDetails.PhoneNumber;
                encounterFormToUpdate.Email = EncounterFormDetails.Email;   
                encounterFormToUpdate.PresentIllnessHistory = EncounterFormDetails.PresentIllnessHistory;
                encounterFormToUpdate.MedicalHistory = EncounterFormDetails.MedicalHistory;
                encounterFormToUpdate.Medications = EncounterFormDetails.Medications;
                encounterFormToUpdate.Allergies = EncounterFormDetails.Allergies;
                encounterFormToUpdate.Temperature = EncounterFormDetails.Temperature;
                encounterFormToUpdate.HeartRate = EncounterFormDetails.HeartRate;
                encounterFormToUpdate.RespirationRate = EncounterFormDetails.RespirationRate;
                encounterFormToUpdate.BloodPressureSystolic = EncounterFormDetails.BloodPressureSystolic;
                encounterFormToUpdate.BloodPressureDiastolic = EncounterFormDetails.BloodPressureDiastolic;
                encounterFormToUpdate.OxygenLevel = EncounterFormDetails.OxygenLevel;
                encounterFormToUpdate.Pain = EncounterFormDetails.Pain;
                encounterFormToUpdate.Heent = EncounterFormDetails.Heent;
                encounterFormToUpdate.Cardiovascular = EncounterFormDetails.Cardiovascular;
                encounterFormToUpdate.Chest = EncounterFormDetails.Chest;
                encounterFormToUpdate.Abdomen = EncounterFormDetails.Abdomen;
                encounterFormToUpdate.Extremities = EncounterFormDetails.Extremities;
                encounterFormToUpdate.Skin = EncounterFormDetails.Skin;
                encounterFormToUpdate.Neuro = EncounterFormDetails.Neuro;
                encounterFormToUpdate.Other = EncounterFormDetails.Other;
                encounterFormToUpdate.Diagnosis = EncounterFormDetails.Diagnosis;
                encounterFormToUpdate.TreatmentPlan = EncounterFormDetails.TreatmentPlan;
                encounterFormToUpdate.MedicationsDispensed = EncounterFormDetails.MedicationsDispensed;
                encounterFormToUpdate.Procedures = EncounterFormDetails.Procedures;
                encounterFormToUpdate.FollowUp = EncounterFormDetails.FollowUp;
                
                await _commonRepository.UpdateEncounterForm(encounterFormToUpdate);

                return true;
            }

            return false;

        }

        public async Task<bool> SendLink(string receiverEmail)
        {
            string subject = "Get Medical Assistance @HalloDoc";

            string url = "http://localhost:5059/RequestForms/SubmitRequest";
            string body = "Click on the link and create a new request to get the medical assistance for you or your family : " + url;

            List<string> receivers = new List<string>
                {
                    receiverEmail
                };

            bool isMailSent = await _mailService.SendMail(receivers, subject, body, false, new List<string>());
            return isMailSent;
        }

        public byte[] ExportToExcel(int requestStatus, int? requestType, string? searchPattern, int? searchRegion, int? pageNumber)
        {
            var requests = _requestRepository.GetAllIEnumerableRequests().AsQueryable();

            int[] myArray = new CommonMethods().GetRequestStatus(requestStatus);

            requests = requests.AsQueryable().Include(x => x.RequestClients).Include(x => x.Physician).Include(x => x.RequestStatusLogs).Where(x => myArray.Contains(x.Status));


            if (requestType != null)
            {
                requests = requests.Where(x => x.RequestTypeId == requestType);
            }

            if (searchPattern != null)
            {
                requests = requests.Where(x => EF.Functions.Like(x.RequestClients.FirstOrDefault().FirstName, "%" + searchPattern + "%"));
            }

            if (searchRegion != null)
            {
                requests = requests.Where(x => x.RequestClients.FirstOrDefault().RegionId == searchRegion);
            }

            if(pageNumber != null)
            {
                int requestCount = requests.Count();
                int pageSize = 5;
                if (pageNumber <= 0)
                {
                    pageNumber = 1;
                }

                int requestToSkip = ((pageNumber ?? 1) - 1) * pageSize;

                requests = requests.Skip(requestToSkip).Take(pageSize);
            }

            var requestData = requests.ToList();

            DataTable dataTable = new DataTable("Requests");

            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("RequestId"),
                new DataColumn("Patient Name"),
                new DataColumn("Date of Birth"),
                new DataColumn("Requestor"),
                new DataColumn("Requested Date"),
                new DataColumn("Physician Name"),
                new DataColumn("Date of Service"),
                new DataColumn("Phone Number"),
                new DataColumn("Address"),
                new DataColumn("Note"),

            });

            foreach(var row in requestData)
            {
                int requestId = row.RequestId;
                string patientName = row.RequestClients.FirstOrDefault()?.FirstName + " " + row.RequestClients.FirstOrDefault()?.LastName;

                RequestClient requestClient = row.RequestClients.FirstOrDefault() ?? new RequestClient();
                string DOB = "";
                if (requestClient.IntDate.HasValue && requestClient.IntYear.HasValue && requestClient.StrMonth != null)
                {
                    DateTime monthDateTime = DateTime.ParseExact(requestClient.StrMonth, "MMMM", CultureInfo.InvariantCulture);
                    int month = monthDateTime.Month;
                    DateOnly date = new DateOnly((int)requestClient.IntYear, month, requestClient.IntDate.Value);
                    DOB = date.ToString("yyyy-MM-dd");
                }

                string requestor = row.FirstName + " " + row.LastName;
                string requestDate = row.CreatedDate.ToString();
                string physicianName = row.Physician?.FirstName + " " + row.Physician?.LastName;
                string dateOfService = row.AcceptedDate.ToString() ?? "";
                string phoneNumber = row.RequestClients.FirstOrDefault()?.PhoneNumber ?? "";
                string address = row.RequestClients.FirstOrDefault()?.Address ?? "";
                string note = row.RequestStatusLogs.FirstOrDefault()?.Notes ?? "";

                dataTable.Rows.Add(requestId, patientName, DOB, requestor, requestDate, physicianName, dateOfService, phoneNumber, address, note);

                
            }

            using(XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using(MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    return stream.ToArray();
                }
            }
        }
    }
}
