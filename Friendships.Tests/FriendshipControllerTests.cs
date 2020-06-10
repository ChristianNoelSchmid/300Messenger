using Friendships.Controllers;
using Friendships.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using Shared.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Friendships.Tests
{
    public class FriendshipControllerTests
    {
        private FriendshipController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new FriendshipController(
                new MockFriendshipRepo(),
                null
            );
        }

        [Test]
        public async Task TestAddingAlreadyExistingFriendship()
        {
            var response = await _controller.Create(
                new Shared.ViewModels.AuthorizedEmailViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}geddy@fakemail.com",
                    Email = "alex@fakemail.com"
                }
            );

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            Assert.AreEqual("Friendship already exists", ((BadRequestObjectResult)response).Value);
        }

        [Test]
        public async Task TestAddingNewFriendship()
        {
            var response = await _controller.Create(
                new Shared.ViewModels.AuthorizedEmailViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}alex@fakemail.com",
                    Email = "neil@fakemail.com"
                }
            );

            Assert.IsInstanceOf<OkResult>(response);
        }

        [Test]
        public async Task TestGettingAllFriendships()
        {
            var response = await _controller.GetFriendships(
                new Shared.ViewModels.AuthorizedJwtViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}geddy@fakemail.com"
                });

            Assert.IsInstanceOf<OkObjectResult>(response);

            var friendships = JsonConvert.DeserializeObject<Friendship[]>(
                ((OkObjectResult)response).Value.ToString()
            );

            Assert.AreEqual("alex@fakemail.com", friendships.FirstOrDefault(f => f.Id == 1).RequesterEmail);
            Assert.AreEqual("geddy@fakemail.com", friendships.FirstOrDefault(f => f.Id == 1).ConfirmerEmail);
            Assert.AreEqual(true, friendships.FirstOrDefault(f => f.Id == 1).IsConfirmed);

            Assert.AreEqual("neil@fakemail.com", friendships.FirstOrDefault(f => f.Id == 2).RequesterEmail);
            Assert.AreEqual("geddy@fakemail.com", friendships.FirstOrDefault(f => f.Id == 2).ConfirmerEmail);
        }

        [Test]
        public async Task TestConfirmingFriendship()
        {
            var response = await _controller.Confirm(
                new Shared.ViewModels.AuthorizedIntViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}neil@fakemail.com",
                    Value = 2
                }
            );
            Assert.IsInstanceOf<BadRequestObjectResult>(response);

            response = await _controller.Confirm(
                new Shared.ViewModels.AuthorizedIntViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}geddy@fakemail.com",
                    Value = 2
                }
            );

            Assert.IsInstanceOf<OkResult>(response);

            response = await _controller.GetFriendship(
                new Shared.ViewModels.AuthorizedEmailViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}geddy@fakemail.com",
                    Email = "neil@fakemail.com"
                }
            );

            Assert.IsInstanceOf<OkObjectResult>(response);
            var friendship = JsonConvert.DeserializeObject<Friendship>(((OkObjectResult)response).Value.ToString());

            Assert.AreEqual(true, friendship.IsConfirmed);
        }  
    }
}