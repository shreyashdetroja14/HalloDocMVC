using System.ComponentModel.DataAnnotations;

namespace HalloDocServices.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email cannot be empty")]
        //[EmailAddress(ErrorMessage ="Enter a valid email address")]
        //[DataType(DataType.EmailAddress)]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; } = null!;
    }
}
