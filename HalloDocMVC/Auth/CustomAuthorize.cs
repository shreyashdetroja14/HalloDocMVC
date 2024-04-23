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
        private readonly string[] _roles;

        public CustomAuthorize(params string[] roles)
        {
            _roles = roles;
        }
        /*private readonly IList<string> _role;

        public CustomAuthorize(params string[] role)
        {
            _role = role;
        }*/

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();

            if (jwtService == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            var request = context.HttpContext.Request;
            var token = request.Cookies["jwt"];

            if (token == null)
            {
                if (context.HttpContext.Request.Headers.TryGetValue("X-Requested-With", out var requestedWith) && requestedWith.FirstOrDefault() == "XMLHttpRequest")
                {
                    context.Result = new BadRequestObjectResult(new ProblemDetails
                    {
                        Status = 414
                    });
                    return;
                }

                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
                return;
            }

            if (!jwtService.ValidateToken(token, out JwtSecurityToken jwtToken))
            {
                if (context.HttpContext.Request.Headers.TryGetValue("X-Requested-With", out var requestedWith) && requestedWith.FirstOrDefault() == "XMLHttpRequest")
                {
                    context.Result = new BadRequestObjectResult(new ProblemDetails
                    {
                        Status = 414
                    });
                    return;
                }

                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Logout" }));
                return;
            }

            /*if (request.Path.Value == "/Login" || request.Path.Value == "/Login/Index")
            {
                var id = jwtToken.Claims.FirstOrDefault(x => x.Type == "aspnetuserId")?.Value;
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Patient", action = "Dashboard", id = id }));
                return;
            }*/

            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

            if (roleClaim == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Logout" }));
                return;
            }

            bool isAuthorized = false;
            foreach (var role in _roles)
            {
                if (roleClaim.Value == role)
                {
                    isAuthorized = true;
                    break;
                }
            }

            if (!isAuthorized)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
            }

            /*if(string.IsNullOrWhiteSpace(_role) ||roleClaim.Value != _role)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "AccessDenied" }));
            }*/
        }


    }
}
