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
    /// <summary>
    /// The Controller for the Accounts microservice.
    /// Handles all incoming Http Requests regarding User registration,
    /// logging in, and verifying Jwts. Also handles confirmation emails,
    /// and validating Users emails before they can login (NOTE: feature
    /// currently does not work on server)
    /// </summary>
    [ApiController]
    [Route("/")]
    public class AccountController : Controller
    {
        private readonly IUserRepo userRepo;
        private readonly IToConfirmRepo toConfirmRepo;
        private readonly ITokenBuilder tokenBuilder;
        private readonly IMailService mailService;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<AccountController> logger;

        public AccountController(IUserRepo userRepo,
                                 IToConfirmRepo toConfirmRepo,
                                 ITokenBuilder tokenBuilder,
                                 IMailService mailService,
                                 IHttpClientFactory clientFactory,
                                 ILogger<AccountController> logger)
        {
            this.userRepo = userRepo;
            this.toConfirmRepo = toConfirmRepo;
            this.tokenBuilder = tokenBuilder;
            this.mailService = mailService;
            this.clientFactory = clientFactory;
            this.logger = logger;
        }

        /// <summary>
        /// Creates a list of fake Users in the database. The
        /// Users cannot be seeded during intialization of the database, due
        /// to the need for a password hasher. Therefore, it's done after, by
        /// Http request.
        /// </summary>
        /// <param name="viewModel">
        /// The viewModel which contains the Secret
        /// associated with the application. Found in Services.AuthorizationServices
        /// </param>
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

                //List<int> tokens = new List<int>();
                //await context?.ToConfirms.ForEachAsync(tc => tokens.Add(tc.Token));
                //foreach (var token in tokens) await ConfirmEmail(token);

                return Ok();
            }
            return BadRequest();
        }

        /// <summary>
        /// Registers a new User, provided the information supplied in the viewModel
        /// is correct.
        /// Users MUST have...
        ///     1) A unique email
        ///     2) Two matching passwords (for confirmation)
        ///     3) A first and last name inputted
        ///  This method also sends a confirmation email to the User (NOTE: does not work on
        ///  server right now)
        /// </summary>
        /// <param name="viewModel">The information the registering User inputted</param>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                // Create the user
                var user = new User()
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    EmailConfirmed = true
                };

                try
                {
                    // Add them to the database
                    await userRepo.CreateUserAsync(user, viewModel.Password);
                }
                catch(UserAlreadyExistsException)
                {
                    // If the email matches a different User, return a BadRequest
                    return BadRequest($"User with email {user.Email} already exists");
                }

                logger?.LogInformation($"User created: {user.Email}");

                // Not Working On Server for now
                // var toConfirm = await toConfirmRepo.AddAsync(user.Email);
                // mailService?.SendConfirmationEmail(toConfirm);

                return Ok("User created: please confirm email to continue");
            }

            return ValidationProblem(ModelState);
        }

        /// <summary>
        /// Logs in the User. Requires an email and password to login
        /// <param name="viewModel">The login information</param>
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
                    // Attempt to Login
                    var user = await userRepo.LoginUserAsync(viewModel.Email, viewModel.Password);

                    // Disallow logging in if the email has not been
                    // confirmed by the User yet
                    if(!user.EmailConfirmed)
                    {
                        return Unauthorized("Email must be confirmed before proceeding.");
                    }
                    
                    logger?.LogInformation($"User JWT accessed: {user.Email}");
                    return Ok(tokenBuilder.BuildToken(user.Email));
                }
                // BadRequest if User does not exist
                catch(UserDoesNotExistException)
                {
                    return BadRequest($"The User with email {viewModel.Email} does not exist");
                }
                // Bad request if password does not match hashed password
                catch(UserPasswordDoesNotMatchException)
                {
                    return BadRequest($"The password did not match");
                }
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Confirms the supplied Email address, by the token stored in
        /// the ToConfirms database
        /// <param name="token">The token representing the confirmed email</param>
        /// </summary>
        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(int token)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    // Retrieve the ToConfirm
                    var toConfirm = await toConfirmRepo.GetToConfirmAsync(token);

                    // If a token exists, update the User to have a confirmed email
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

        /// <summary>
        /// Retrives the User, using the supplied Jwt
        /// </summary>
        /// <param name="viewModel">The Jwt view model</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserByJwt")]
        public async Task<IActionResult> GetUser(AuthorizedJwtViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Verify the Jwt
                var fromEmail =
                    await AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

                // Return Unauthorized if the Jwt was not validated
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


        /// <summary>
        /// Retrieves a User by email address. Requires an authorized Jwt
        /// </summary>
        /// <param name="viewModel">The view model containing the Jwt and Email</param>
        [HttpGet]
        [Route("GetUserByEmail")]
        public async Task<IActionResult> GetUser(AuthorizedEmailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Verify the Jwt
                var fromEmail =
                    await AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

                // Return Unauthorized if the Jwt was not validated
                if (fromEmail == null)
                    return Unauthorized();

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

        /// <summary>
        /// Returns all users under the supplied query
        /// Searches by first and last name, and email
        /// Includes the User provided by the Jwt, so clients must
        /// consider that additional element.
        /// </summary>
        /// <param name="viewModel">The Jwt-verified search query</param>
        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers(AuthorizedQueryViewModel viewModel)
        {
            var values = new List<string>();
            var query = viewModel.Value;

            // Verify the Jwt
            var fromEmail = 
                await AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

            if(fromEmail != null)
            {
                // Search through the repository
                if(query != null)
                {
                    // Split all elements by spaces, tabs, and newlines
                    // and search through each one
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