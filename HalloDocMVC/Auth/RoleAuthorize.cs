using HalloDocServices.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;

namespace HalloDocMVC.Auth
{
    [AttributeUsage(AttributeTargets.All)]
    public class RoleAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly string[] _menus;

        public RoleAuthorize(params string[] menus)
        {
            _menus = menus;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            var roleAuthService = context.HttpContext.RequestServices.GetService<IRoleAuthService>();

            if (jwtService == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }
            
            if (roleAuthService == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            var request = context.HttpContext.Request;
            var token = request.Cookies["jwt"];

            if (token == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            if (jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                int roleId = int.Parse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == "roleId")?.Value ?? "0");
                bool isAccess = roleAuthService.CheckAccess(roleId, _menus);
                if (!isAccess)
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
                }
            }
            else
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Logout" }));
                return;
            }
        }
    }
}
