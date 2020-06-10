using _300Messenger.Shared.ViewModels;
using Accounts.Controllers;
using Accounts.Models;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Services;
using Shared.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Accounts.Tests
{
    public class AccountControllerTests
    {
        private AccountController controller;

        [SetUp]
        public void SetUp()
        {
            controller = new AccountController(
                null, new MockUserRepo(), new MockToConfirmRepo(),
                new TokenBuilder(), null, null, null
            );
        }

        [Test]
        public async Task TestAlreadyExistsRegistration()
        {
            var response = await controller.Register(
                new RegisterViewModel
                {
                    Email = "roger@fakemail.com",
                    Password = "password",
                    ConfirmPassword = "password",
                    FirstName = "Roger",
                    LastName = "Daltry"
                }
            );

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
            var result = (BadRequestObjectResult)response;

            Assert.AreEqual("User with email roger@fakemail.com already exists", result.Value);
        }

        [Test]
        public async Task TestToConfirmUponRegistration()
        {
            var response = await controller.Register(
                new RegisterViewModel
                {
                    Email = "alex@fakemail.com",
                    Password = "password",
                    ConfirmPassword = "password",
                    FirstName = "Alex",
                    LastName = "Lifeson"
                }
            );
            
            Assert.IsInstanceOf<OkObjectResult>(response);
            Assert.IsInstanceOf<OkObjectResult>(await controller.ConfirmEmail("alex@fakemail.com".GetHashCode()));
            Assert.IsInstanceOf<OkObjectResult>(
                await controller.Login(new LoginViewModel { Email = "alex@fakemail.com", Password = "password" })
            );
            Assert.IsInstanceOf<UnauthorizedObjectResult>(
                await controller.Login(new LoginViewModel { Email = "geddy@fakemail.com", Password = "password" })
            );
        }

        [Test]
        public async Task VerifyJwtTest()
        {
            var response = 
                (OkObjectResult)await controller.Login(
                new LoginViewModel
                {
                    Email = "roger@fakemail.com",
                    Password = "password"
                }
            );

            Assert.IsInstanceOf<string>(response.Value);
        }
    }
}