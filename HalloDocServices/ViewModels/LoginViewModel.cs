using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels
{
    public class LoginViewModel
    {
        //[Required(ErrorMessage = "Please Enter Email Address")]
        //[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}))$", ErrorMessage = "Please enter a valid email address. (e.g., user@example.com)")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; } = null!;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
    }
}
