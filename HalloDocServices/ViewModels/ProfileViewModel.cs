namespace HalloDocServices.ViewModels
{
    public class ProfileViewModel
    {
        public int? UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string? LastName { get; set; }

        public string DOB { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        public int? PhoneNumberType { get; set; }

        public string Email { get; set; } = null!;

        public string? Street { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? ZipCode { get; set; }
    }
}
