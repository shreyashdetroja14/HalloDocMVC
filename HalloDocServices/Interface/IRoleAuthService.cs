using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IRoleAuthService
    {
        bool CheckAccess(int roleId, string[] menus);
    }
}
