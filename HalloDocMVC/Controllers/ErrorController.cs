using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found.";
                    ViewBag.Path = statusCodeResult?.OriginalPath;
                    ViewBag.QS = statusCodeResult?.OriginalQueryString;
                    break;
            }

            _logger.LogWarning($"404 Error occured. Path = {statusCodeResult?.OriginalPath} and QueryString = {statusCodeResult?.OriginalQueryString}");

            return View("NotFound");
        }

        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerFeature>();

            ViewBag.ExceptionPath = exceptionDetails?.Path;
            ViewBag.ExceptionDetails = exceptionDetails?.Error.Message;
            ViewBag.StackTrace = exceptionDetails?.Error.StackTrace;

            _logger.LogError($"The path {exceptionDetails?.Path} threw an exception {exceptionDetails?.Error}");

            return View("Error");
        }
    }
}
