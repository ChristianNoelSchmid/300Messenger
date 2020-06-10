using Messages.Controllers;
using Messages.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using Shared.Models;
using System.Threading.Tasks;

namespace Messages.Tests
{
    public class MessageSessionControllerTests
    {
        private MessageSessionController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new MessageSessionController
                (new MockMessageSessionRepo(), null);
        }

        [Test]
        public async Task TestGetSessionWhileNotAssociated()
        {
            var response = await _controller.GetSession(
                new Shared.ViewModels.AuthorizedIntViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}pete@fakemail.com",
                    Value = 1
                }
            );

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task TestGetSessionWithAssociated()
        {
            var response = await _controller.GetSession(
                new Shared.ViewModels.AuthorizedIntViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}pete@fakemail.com",
                    Value = 2
                }
            );

            Assert.IsInstanceOf<OkObjectResult>(response);

            var session = JsonConvert.DeserializeObject<MessageSession>(
                ((OkObjectResult)response).Value.ToString()
            );

            Assert.AreEqual("Pizza", session.Title);
        }

        [Test]
        public async Task TestAddMessageToSessionWhileNotAssociated()
        {
            var response = await _controller.AddMessageToSession(
                new Shared.ViewModels.AuthorizedMessageViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}pete@fakemail.com",
                    SessionId = 1,
                    Content = "Yeah! Food!",
                    Type = MessageType.Text
                }
            );

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }
        
        [Test]
        public async Task TestAddMessageToSessionWhileAssociated()
        {
            var response = await _controller.AddMessageToSession(
                new Shared.ViewModels.AuthorizedMessageViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}pete@fakemail.com",
                    SessionId = 2,
                    Content = "Yeah! Food!",
                    Type = MessageType.Text
                }
            );

            Assert.IsInstanceOf<OkObjectResult>(response);

            var session = JsonConvert.DeserializeObject<MessageSession>(
                ((OkObjectResult)response).Value.ToString()
            );

            Assert.AreEqual("Pizza", session.Title);

            response = await _controller.GetMessagesForSession(
                new Shared.ViewModels.AuthorizedIntViewModel
                {
                    JwtFrom = $"{Shared.Authorization.SECRET}pete@fakemail.com",
                    Value = 2
                }
            );

            var messages = JsonConvert.DeserializeObject<Message[]>(((OkObjectResult)response).Value.ToString());

            Assert.AreEqual("Yeah! Food!", messages[2].Content);
        }
    }
}