using HalloDocEntities.Models;
using HalloDocServices.Interface;
using HalloDocServices.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Implementation
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateJwtToken(AspNetUser aspNetUser)
        {
            int id = 0;
            int roleId = 0;
            string role = aspNetUser.AspNetUserRoles.FirstOrDefault()?.Role.Name ?? "";
            if(role == "admin")
            {
                id = aspNetUser.AdminAspNetUsers.FirstOrDefault()?.AdminId ?? 0;
                roleId = aspNetUser.AdminAspNetUsers.FirstOrDefault()?.RoleId ?? 0;
            }
            else if(role == "physician")
            {
                id = aspNetUser.PhysicianAspNetUsers.FirstOrDefault()?.PhysicianId ?? 0;
                roleId = aspNetUser.PhysicianAspNetUsers.FirstOrDefault()?.RoleId ?? 0;
            }
            else if(role == "patient")
            {
                id = aspNetUser.Users.FirstOrDefault()?.UserId ?? 0;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, aspNetUser.Email ?? ""),
                new Claim(ClaimTypes.Role, aspNetUser.AspNetUserRoles.FirstOrDefault()?.Role.Name ?? ""),
                new Claim("aspnetuserId", aspNetUser.Id),
                new Claim("username", aspNetUser.UserName),
                new Claim("roleId", roleId.ToString()),
                new Claim("id", id.ToString()),

                
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(15);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = null;

            if(token == null)
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]??"");
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                jwtSecurityToken = (JwtSecurityToken)validatedToken;

                if(jwtSecurityToken != null)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateEmailToken(string email, bool isExpireable)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = isExpireable ? DateTime.UtcNow.AddHours(1) : DateTime.MaxValue;

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsData GetClaimValues()
        {
            ClaimsData claimsData = new ClaimsData();

            var Request = _httpContextAccessor.HttpContext?.Request;
            string token = Request?.Cookies["jwt"] ?? "";

            if (ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                claimsData.AspNetUserId = jwtToken.Claims.FirstOrDefault(x => x.Type == "aspnetuserId")?.Value;
                claimsData.Email = jwtToken?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                claimsData.AspNetUserRole = jwtToken?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                claimsData.Username = jwtToken?.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
                claimsData.RoleId = int.Parse(jwtToken?.Claims.FirstOrDefault(x => x.Type == "roleId")?.Value ?? "0");
                claimsData.Id = int.Parse(jwtToken?.Claims.FirstOrDefault(x => x.Type == "id")?.Value ?? "");
            }

            return claimsData;
        }
    }
}
