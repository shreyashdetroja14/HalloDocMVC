using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email cannot be empty")]
        [RegularExpression(@"^[\w!#$%&'*+/=?^`{|}~-]+(?:\.[\w!#$%&'*+/=?^`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]

        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password can't be empty")]
        public string Password { get; set; } = null!;
    }
}
