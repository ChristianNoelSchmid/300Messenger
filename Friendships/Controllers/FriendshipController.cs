using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using _300Messenger.Friendships.Models;
using _300Messenger.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _300Messenger.Friendships.Controllers
{
    [Route("[controller]")]
    public class FriendshipController : Controller
    {
        private readonly IFriendshipRepo friendshipRepo;
        private readonly HttpClient verifyClient;
        private readonly ILogger<FriendshipController> logger;

        public FriendshipController(IFriendshipRepo friendshipRepo, 
            IHttpClientFactory clientFactory, ILogger<FriendshipController> logger)
        {
            this.friendshipRepo = friendshipRepo;
            verifyClient = Shared.Services.Authorization.CreateVerificationClient(clientFactory);
            this.logger = logger;
        }

        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> CreateFriendship(FriendshipViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = await 
                    Shared.Services.Authorization.VerifyToken(verifyClient, viewModel.FromJwtToken);

                logger.LogInformation(verifyClient.DefaultRequestHeaders.ToString());

                if(fromEmail != null)
                {
                    await friendshipRepo.AddUnconfirmedFriendship(fromEmail, viewModel.ToEmail);
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPatch]
        [Route("Confirm")]
        public async Task<IActionResult> ConfirmFriendship(int id)
        {
            if(ModelState.IsValid)
            {
                await friendshipRepo.ConfirmFriendship(id);
            }

            return BadRequest();
        }
    }
}