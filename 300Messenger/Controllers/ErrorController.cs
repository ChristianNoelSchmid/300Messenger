using _300Messenger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _300Messenger.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }
        
        [Route("Error/{code}")]
        public IActionResult HttpStatusCodeHandler(int code)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch(code)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found.";
                    logger.LogWarning($"404 Error Occured. Path = {statusCodeResult.OriginalPath} " + 
                                      $"and QueryString = {statusCodeResult.OriginalQueryString}");
                    break;
            }

            return View("NotFound");
        } 

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var viewModel = new ErrorViewModel()
            {
                RequestId = HttpContext.Request.Path
            };

            logger.LogError($"The path {exceptionDetails.Path} " + 
                            $"threw an exception {exceptionDetails.Error}");

            return View(viewModel);
        }
    }
}