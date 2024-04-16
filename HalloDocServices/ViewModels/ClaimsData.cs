using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels
{
    public class ClaimsData
    {
        public string? AspNetUserId { get; set; }

        public string? Email { get; set; }

        public string? AspNetUserRole { get; set; }
        
        public string? Username { get; set; }

        public int RoleId { get; set; }

        public int AdminId { get; set; }

        public int UserId { get; set; }
        
        public int PhysicianId { get; set;}

        public int Id { get; set; }
    
    }
}
