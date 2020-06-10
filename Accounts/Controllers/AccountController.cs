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
using Accounts.Models;
using Accounts.Exceptions;
using _300Messenger.Shared.ViewModels;
using Services;
using Accounts.Tools;
using Shared.ViewModels;
using Shared.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly AppDbContext context;
        private readonly IUserRepo userRepo;
        private readonly IToConfirmRepo toConfirmRepo;
        private readonly ITokenBuilder tokenBuilder;
        private readonly IMailService mailService;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<AccountController> logger;

        public AccountController(AppDbContext context,
                                 IUserRepo userRepo,
                                 IToConfirmRepo toConfirmRepo,
                                 ITokenBuilder tokenBuilder,
                                 IMailService mailService,
                                 IHttpClientFactory clientFactory,
                                 ILogger<AccountController> logger)
        {
            this.context = context;
            this.userRepo = userRepo;
            this.toConfirmRepo = toConfirmRepo;
            this.tokenBuilder = tokenBuilder;
            this.mailService = mailService;
            this.clientFactory = clientFactory;
            this.logger = logger;
        }

        [HttpPost]
        [Route("Init")]
        public async Task<IActionResult> Init(SecretViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                if (viewModel.Secret != Shared.Authorization.SECRET)
                    return Unauthorized();

                await Register(new RegisterViewModel
                {
                    Email = "pete@fakemail.com",
                    FirstName = "Pete",
                    LastName = "Townshend",
                    Password = "password",
                    ConfirmPassword = "password"
                });
                await Register(new RegisterViewModel
                {
                    Email = "roger@fakemail.com",
                    FirstName = "Roger",
                    LastName = "Daltrey",
                    Password = "password",
                    ConfirmPassword = "password"
                });
                await Register(new RegisterViewModel
                {
                    Email = "keith@fakemail.com",
                    FirstName = "Keith",
                    LastName = "Moon",
                    Password = "password",
                    ConfirmPassword = "password" 
                });
                await Register(new RegisterViewModel
                {
                    Email = "john@fakemail.com",
                    FirstName = "John",
                    LastName = "Entwistle",
                    Password = "password",
                    ConfirmPassword = "password" 
                });
                await Register(new RegisterViewModel
                {
                    Email = "jimmi@fakemail.com",
                    FirstName = "Jimmi",
                    LastName = "Hendrix",
                    Password = "password",
                    ConfirmPassword = "password" 
                });
                await Register(new RegisterViewModel
                { 
                    Email = "geddy@fakemail.com",
                    FirstName = "Geddy",
                    LastName = "Lee",
                    Password = "password",
                    ConfirmPassword = "password" 
                });
                await Register(new RegisterViewModel
                {
                    Email = "neil@fakemail.com",
                    FirstName = "Neil",
                    LastName = "Peart",
                    Password = "password",
                    ConfirmPassword = "password" 
                });
                await Register(new RegisterViewModel
                {
                    Email = "alex@fakemail.com",
                    FirstName = "Alex",
                    LastName = "Lifeson",
                    Password = "password",
                    ConfirmPassword = "password"
                });
                await Register(new RegisterViewModel
                {
                    Email = "freddie@fakemail.com",
                    FirstName = "Freddie",
                    LastName = "Mercury",
                    Password = "password",
                    ConfirmPassword = "password"  
                });

                List<int> tokens = new List<int>();
                await context?.ToConfirms.ForEachAsync(tc => tokens.Add(tc.Token));
                foreach (var token in tokens) await ConfirmEmail(token);

                return Ok();
            }
            return BadRequest();
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
                    Email = viewModel.Email,
                };

                try
                {
                    await userRepo.CreateUserAsync(user, viewModel.Password);
                }
                catch(UserAlreadyExistsException)
                {
                    return BadRequest($"User with email {user.Email} already exists");
                }

                logger?.LogInformation($"User created: {user.Email}");

                var toConfirm = await toConfirmRepo.AddAsync(user.Email);
                mailService?.SendConfirmationEmail(toConfirm);

                return Ok("User created: please confirm email to continue");
            }

            return ValidationProblem(ModelState);
        }

        /// <summary>
        /// Logs in the User, 
        /// <return>The Jwt Token, if authentication was successful</return>
        /// </summary>
        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var user = await userRepo.LoginUserAsync(viewModel.Email, viewModel.Password);
                    if(!user.EmailConfirmed)
                    {
                        return Unauthorized("Email must be confirmed before proceeding.");
                    }
                    
                    logger?.LogInformation($"User JWT accessed: {user.Email}");
                    return Ok(tokenBuilder.BuildToken(user.Email));
                }
                catch(UserDoesNotExistException)
                {
                    return BadRequest($"The User with email {viewModel.Email} does not exist");
                }
                catch(UserPasswordDoesNotMatchException)
                {
                    return BadRequest($"The password did not match");
                }
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Confirms the supplied Email address (in viewModel)
        /// </summary>
        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(int token)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var toConfirm = await toConfirmRepo.GetToConfirmAsync(token);
                    if (toConfirm != null)
                    {
                        var user = await userRepo.GetUserAsync(toConfirm.EmailToConfirm);

                        user.EmailConfirmed = true;
                        await userRepo.UpdateUserInfoAsync(user);
                        await toConfirmRepo.RemoveAsync(toConfirm.Id);

                        return Ok("User Email Confirmed");
                    }
                    else return BadRequest("Token not recognized");
                }
                catch(UserDoesNotExistException)
                {
                    return BadRequest($"User with token {token} does not exist.");
                }
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
            // Retrieve the single claim from the JWT (Email)
            var email = User.Claims.SingleOrDefault();

            if(email == null)
            {
                return BadRequest("Token could not be validated");
            }

            try
            {
                var user = await userRepo.GetUserAsync(email.Value);
                return Ok(email.Value);
            }
            catch(UserDoesNotExistException)
            {
                return Unauthorized("Token does not match any User in database");
            }
        }

        [HttpGet]
        [Route("GetUserByJwt")]
        public async Task<IActionResult> GetUser(AuthorizedJwtViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var fromEmail =
                    await Services.AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

                if (fromEmail == null)
                    return Unauthorized();

                try
                {
                    return Ok(JsonConvert.SerializeObject(await userRepo.GetUserAsync(fromEmail)));
                }
                catch(UserDoesNotExistException)
                {
                    return BadRequest();
                }
            }

            return BadRequest(ModelState.Values);
        }


        [HttpGet]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUser(AuthorizedEmailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return Ok(JsonConvert.SerializeObject(await userRepo.GetUserAsync(viewModel.Email)));
                }
                catch (UserDoesNotExistException)
                {
                    return BadRequest("The User does not exist");
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers(AuthorizedQueryViewModel viewModel)
        {
            var values = new List<string>();
            var query = viewModel.Value;

            var fromEmail = 
                await AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

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
                    userRepo.Where(
                        u =>
                           values.Contains(u.Email.ToLower()) 
                        || values.Contains(u.FirstName.ToLower())
                        || values.Contains(u.LastName.ToLower())
                    )
                );
            }
            return Unauthorized("User token not authorized.");
        }
    }
}