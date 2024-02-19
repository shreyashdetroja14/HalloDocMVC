namespace HalloDocEntities.ViewModels
{
    public class CreateAccountViewModel
    {
        public string Email { get; set; } = null!; 
        public string Password { get; set; } = null!;

        public string ConfirmPassword { get; set; } = null!;
    }
}
