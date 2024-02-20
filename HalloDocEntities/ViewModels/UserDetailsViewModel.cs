using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HalloDocEntities.ViewModels
{
    public class UserDetailsViewModel
    {
        public string? AspNetUserId { get; set; }

        public int UserId { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
