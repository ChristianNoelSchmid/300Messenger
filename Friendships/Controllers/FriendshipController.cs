using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Friendships.Exceptions;
using Friendships.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Models;
using Shared.ViewModels;

namespace Friendships.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FriendshipController : Controller
    {
        private readonly IFriendshipRepo friendshipRepo;
        private readonly IHttpClientFactory _clientFactory;

        public FriendshipController(IFriendshipRepo friendshipRepo, 
            IHttpClientFactory clientFactory)
        {
            this.friendshipRepo = friendshipRepo;
            this._clientFactory = clientFactory;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(AuthorizedEmailViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                // Ensure that the JWT refers to a User in the Repository, using
                // the Accounts microservice
                var fromEmail = 
                    await Services.AuthorizationServices.VerifyToken(_clientFactory, viewModel.JwtFrom);

                if(fromEmail != null)
                {
                    try
                    {
                        await friendshipRepo.AddUnconfirmedFriendshipAsync(fromEmail, viewModel.Email);
                        return Ok();
                    }
                    catch(FriendshipAlreadyExistsException)
                    {
                        return BadRequest("Friendship already exists");
                    }
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPut]
        [Route("Confirm")]
        public async Task<IActionResult> Confirm(AuthorizedIntViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // In the case of confirmation, the JWT email is the user confirming the friendship
                // rather than the requester (see method Create())
                var fromEmail =
                    await Services.AuthorizationServices.VerifyToken(_clientFactory, viewModel.JwtFrom);

                if (fromEmail != null)
                {
                    try
                    {
                        var friendship = await friendshipRepo.ConfirmFriendshipAsync(viewModel.Value, fromEmail);
                        return Ok();
                    }
                    catch (FriendshipDoesNotExistException)
                    {
                        return BadRequest("Friendship does not exist");
                    }
                    catch (UnauthorizedFriendConfirmException)
                    {
                        return BadRequest("Friendship cannot be authorized with token");
                    }
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("Remove")]
        public async Task<IActionResult> Remove(AuthorizedIntViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                // In the case of confirmation, the JWT email is the user confirming the friendship
                // rather than the requester (see method Create())
                var fromEmail = 
                    await Services.AuthorizationServices.VerifyToken(_clientFactory, viewModel.JwtFrom);

                if(fromEmail != null)
                {
                    try
                    {
                        await friendshipRepo.RemoveFriendshipAsync(viewModel.Value);
                        return Ok();
                    }
                    catch(FriendshipDoesNotExistException)
                    {
                        return BadRequest("Friendship does not exist");
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("GetFriendship")]
        public async Task<IActionResult> GetFriendship(AuthorizedEmailViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail =
                    await Services.AuthorizationServices.VerifyToken(_clientFactory, viewModel.JwtFrom);

                if(fromEmail != null)
                {
                    try
                    {
                        return Ok(JsonConvert.SerializeObject(
                            await friendshipRepo.GetFriendship(fromEmail, viewModel.Email)
                        ));
                    }
                    catch(FriendshipDoesNotExistException)
                    {
                        return BadRequest("Does Not Exist");
                    }
                }
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("GetFriendships")]
        public async Task<IActionResult> GetFriendships(AuthorizedJwtViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Services.AuthorizationServices.VerifyToken(_clientFactory, viewModel.JwtFrom);

                if(fromEmail != null)
                {
                    return Ok(
                        JsonConvert.SerializeObject(friendshipRepo.GetAllFriendships(fromEmail))
                    );
                }
            }
            return BadRequest();
        }
    }
}