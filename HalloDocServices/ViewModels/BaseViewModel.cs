using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.ViewModels
{
    public class BaseViewModel
    {

        public int AccountType { get; set; }

        public string? AspNetUserRole { get; set; }

        public string? Username { get; set; }
    }
}
