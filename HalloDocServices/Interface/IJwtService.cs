using HalloDocEntities.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Interface
{
    public interface IJwtService
    {
        string GenerateJwtToken(AspNetUser aspNetUser);

        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);

        string GenerateEmailToken(string email, bool isExpireable);
    }
}
