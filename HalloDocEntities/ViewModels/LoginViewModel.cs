using System.ComponentModel.DataAnnotations;

namespace HalloDocEntities.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email cannot be empty")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; } = null!;
    }
}
