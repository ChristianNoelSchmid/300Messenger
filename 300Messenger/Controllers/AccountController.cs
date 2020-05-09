using System.Threading.Tasks;
using _300Messenger.Shared;
using _300Messenger.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace _300Messenger.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = viewModel.Email,
                    Email = viewModel.Email,
                };

                var result = await userManager.CreateAsync(user, viewModel.Password);

                if(result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }

                // If this section of code is reached, it means that result
                // did not succeed - ie. userManager.CreateAsync was not successful. 
                // If this is the case, add the Errors to the ModelState, which will
                // display in the validation section of the HTML
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            } 
            return View(viewModel);    
        }
    }
}