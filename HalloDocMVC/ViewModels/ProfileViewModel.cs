namespace HalloDocMVC.ViewModels
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public DateOnly DOB { get; set; }

        public string? PhoneNumber { get; set; }

        public int? PhoneNumberType { get; set; }

        public string Email { get; set; } = null!;

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? ZipCode { get; set; }
    }
}
