using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using _300Messenger.Friendships.Models;
using _300Messenger.Friendships.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _300Messenger.Friendships.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FriendshipController : Controller
    {
        private readonly IFriendshipRepo friendshipRepo;
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<FriendshipController> logger;

        public FriendshipController(IFriendshipRepo friendshipRepo, 
            IHttpClientFactory clientFactory, ILogger<FriendshipController> logger)
        {
            this.friendshipRepo = friendshipRepo;
            this.clientFactory = clientFactory;
            this.logger = logger;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(FriendshipViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                // Ensure that the JWT refers to a User in the Repository, using
                // the Accounts microservice
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    await friendshipRepo.AddUnconfirmedFriendship(fromEmail, viewModel.OtherEmail);
                    return Ok();
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPatch]
        [Route("Confirm")]
        public async Task<IActionResult> Confirm(FriendshipViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                // In the case of confirmation, the JWT email is the user confirming the friendship
                // rather than the requester (see method Create())
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                var friendship = await friendshipRepo.ConfirmFriendship(viewModel.OtherEmail, fromEmail);
                if(friendship == null)
                {
                    return BadRequest("Friendship does not exist");
                }
                return Ok();
            }

            return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("GetFriendships")]
        public async Task<IActionResult> GetFriendships(JWTViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    return new JsonResult(
                        friendshipRepo.GetAllFriendships(fromEmail)
                    );
                }
            }
            return BadRequest();
        }
    }
}