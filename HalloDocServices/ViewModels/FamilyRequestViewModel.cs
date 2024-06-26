﻿
using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels
{
    public class FamilyRequestViewModel
    {
        [StringLength(100), MinLength(2, ErrorMessage = "Name can't be a single letter")]
        public string? FamilyFirstName { get; set; }

        public string? FamilyLastName { get; set; }

        /*[RegularExpression(@"^0?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 01234567890 or 9876543210)")]*/
        [RegularExpression(@"^(?:0)?[6789]\d{4}(?:\s?\d{5})?$", ErrorMessage = "Please enter a valid Indian phone number (e.g., 098765 43210, 9876543210)")]
        public string? FamilyPhoneNumber { get; set; }

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string? FamilyEmail { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Relation can only have alphabets")]
        public string? FamilyRelation { get; set; }

        public PatientRequestViewModel PatientInfo { get; set; } = null!;

    }
}
