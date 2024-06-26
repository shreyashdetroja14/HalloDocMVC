﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels
{
    public class AgreementViewModel
    {
        public int RId { get; set; }

        public int? RequestStatus { get; set; }

        public string? PatientFullName { get; set; }

        public string? ConfirmationNumber { get; set; }

        public bool? IsAgreementFilled { get; set; }

        public string? CancellationReason { get; set; }
    }
}
