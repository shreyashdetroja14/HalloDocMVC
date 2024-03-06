using HalloDocServices.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDocMVC.Auth
{
    [AttributeUsage(AttributeTargets.All)]
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly string _role;

        public CustomAuthorize(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();

            if(jwtService == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            var request = context.HttpContext.Request;
            var token = request.Cookies["jwt"];

            if(token == null || !jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Logout" }));
                return;
            }

            if(request.Path.Value == "/Login" || request.Path.Value == "/Login/Index")
            {
                var id = jwtToken.Claims.FirstOrDefault(x => x.Type == "aspnetuserId")?.Value;
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Patient", action = "Dashboard", id = id}));
                return;
            }

            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

            if(roleClaim == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Logout" }));
                return;
            }

            if(string.IsNullOrWhiteSpace(_role) ||roleClaim.Value != _role)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
            }
        }
    }
}
