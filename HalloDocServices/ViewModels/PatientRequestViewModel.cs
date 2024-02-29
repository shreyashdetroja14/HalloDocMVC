using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels
{
    public class PatientRequestViewModel
    {
        public string? Symptoms { get; set; }

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string? DOB { get; set; }

        public string Email { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? ZipCode { get; set; }

        public string? Room { get; set; }

        public string? File { get; set; }

        public IEnumerable<IFormFile>? MultipleFiles { get; set; }
    }
}
