using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Linq;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Net.Http;
using _300Messenger.Accounts.Services;
using _300Messenger.Accounts.Models;
using _300Messenger.Accounts.ViewModels;

namespace _300Messenger.Accounts.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenBuilder tokenBuilder;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<AccountController> logger;

        /// <summary> Used to compare provided Passwords with User Passwords </summary>
        private readonly PasswordHasher<User> hasher;

        public AccountController(UserManager<User> userManager,
                                 ITokenBuilder tokenBuilder,
                                 IHttpClientFactory clientFactory,
                                 ILogger<AccountController> logger)
        {
            ServicePointManager
                .ServerCertificateValidationCallback += 
                (sender, cert, chain, sslPolicyErrors) => true;

            this.userManager = userManager;
            this.tokenBuilder = tokenBuilder;
            this.clientFactory = clientFactory;
            this.logger = logger;
            this.hasher = new PasswordHasher<User>();
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var user = new User()
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    UserName = viewModel.Email,
                    Email = viewModel.Email.ToLower(),
                    EmailConfirmed = false
                };

                var result = await userManager.CreateAsync(user, viewModel.Password);

                if(result.Succeeded)
                {
                    logger.LogInformation($"User created: {user.UserName}");
                    return Ok(tokenBuilder.BuildToken(user.Email));
                }

                foreach(var error in result.Errors)
                {
                    // If this section of code is reached, it means that result
                    // did not succeed - ie. userManager.CreateAsync was not successful. 
                    // If this is the case, add the Errors to the ModelState, which will
                    // display in the validation section of the HTML
                    ModelState.AddModelError("", error.Description);
                }
            }

            return ValidationProblem(ModelState);
        }

        /// <summary>
        /// Logs in the User, using Identity controls
        /// Requires username and password
        /// </summary>
        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(viewModel.Email);
                if(user != null)
                {
                    if(!user.EmailConfirmed)
                    {
                        return Unauthorized("Please confirm email before proceeding");
                    }
                    var result = hasher.VerifyHashedPassword(user, user.PasswordHash, viewModel.Password);
                    if(result == PasswordVerificationResult.Success)
                    {
                        logger.LogInformation($"User JWT accessed: {user.Email}");
                        return Ok(tokenBuilder.BuildToken(user.Email));
                    }
                }
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Confirms the supplied Email address (in viewModel)
        /// </summary>
        [HttpPatch]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(LoginViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(viewModel.Email);
                if(user == null)
                {
                    return BadRequest("User does not exist!");
                }

                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);

                return Ok();
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Verfies the provided Json Web Token (through request header) and matches
        /// with User in database, if they exist.
        /// </summary>
        [HttpGet]
        [Route("Verify")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> VerifyToken()
        {
            logger.LogInformation("Connection Established...");
            // Retrieve the single claim from the JWT (Email)
            var email = User.Claims.SingleOrDefault();

            if(email == null)
            {
                return Unauthorized("Validation Error");
            }

            var user = await userManager.FindByEmailAsync(email.Value);
            if(user == null)
            {
                return Unauthorized("The specified User Could not be found");
            }
            if(!user.EmailConfirmed)
            {
                return Unauthorized("Please confirm email address before proceeding");
            }

            return Ok(user.Email);
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers(AuthorizedQueryViewModel viewModel)
        {
            var values = new List<string>();
            var query = viewModel.Value;

            var fromEmail = 
                await _300Messenger.Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.JwtFrom);

            if(fromEmail != null)
            {
                if(query != null)
                {
                    query = query.ToLower();
                    values.AddRange(query.Split(' ', '\t', '\n'));
                    for(int i = values.Count - 1; i >= 0; i -= 1)
                    {
                        if(values[i].IndexOf(',') != -1) 
                        {
                            values.AddRange(values[i].Split(','));
                            values.RemoveAt(i);
                        }
                    }
                }
                return new JsonResult(
                    userManager.Users.Where(
                        u =>
                            values.Contains(u.Email.ToLower()) 
                        || values.Contains(u.FirstName.ToLower())
                        || values.Contains(u.LastName.ToLower())
                    )
                );
            }
            return BadRequest("User token not authorized.");
        }
    }
}