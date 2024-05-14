using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HalloDocServices.ViewModels.AdminViewModels
{
    public class TimesheetReceiptViewModel
    {
        public int ReceiptId { get; set; }

        public string ReceiptDate { get; set; } = string.Empty;

        public int TimesheetId { get; set; }

        public string? ItemName { get; set; }

        public int? Amount { get; set; }

        public string? FileName { get; set; }

        public string? FilePath { get; set; }

        public IFormFile? FileToUpload { get; set; }

        public int? AdminId { get; set; }

        public int? PhysicianId { get; set; }
    }
}
