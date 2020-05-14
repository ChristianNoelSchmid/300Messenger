using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using _300Messenger.Messages.Exceptions;
using _300Messenger.Messages.Models;
using _300Messenger.Messages.Models.Repositories;
using _300Messenger.Messages.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _300Messenger.Messages.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageSessionController : Controller
    {
        private readonly IMessageSessionRepository sessionRepository;

        private readonly IHttpClientFactory clientFactory;

        // The IWebHostEnvironment interface retrieves information
        // about the hosting environment. It's from this interface that
        // we retrieve the directory to the wwwroot folder 
        // (see Create(MessageCreateViewModel) method)
        private readonly IWebHostEnvironment hostingEnvironment;

        public MessageSessionController(IMessageSessionRepository sessionRepository,
                                 IHttpClientFactory clientFactory,
                                 IWebHostEnvironment hostingEnvironment)
        {
            this.sessionRepository = sessionRepository;
            this.clientFactory = clientFactory;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Route("GetSession")]
        public async Task<IActionResult> GetSession(AuthorizedIntViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    try
                    {
                        var session = await sessionRepository.GetMessageSessionAsync(viewModel.Value, fromEmail); 
                        return Ok(session);
                    }  
                    catch(EmailNotAssociatedWithMessageSessionException)
                    {
                        return BadRequest("Email not associated with requested session");
                    }
                }
            }
            return BadRequest(ModelState);
        }
        
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(MessageSessionCreateViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    var session = new MessageSession
                    {
                        Title = viewModel.Title,
                        Description = viewModel.Description,
                        Emails = string.Join(';', viewModel.Emails).ToLower(),
                        Messages = new List<Message>(),
                    };

                    await sessionRepository.CreateMessageSessionAsync(session);
                    return Ok(session);
                }

                return BadRequest("Specified Token Not Found");
            }
            return BadRequest(ModelState);
        }

        [HttpPost] 
        [Route("Delete")]
        public async Task<IActionResult> Delete(AuthorizedIntViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    try
                    {
                        await sessionRepository.DeleteMessageSessionAsync(viewModel.Value, fromEmail);
                        return Ok();
                    }
                    catch(EmailDoesNotMatchMessageSessionOwnerException) 
                    {
                        return BadRequest("Attempted to delete session not owned by requester.");
                    }
                }
            }

            return BadRequest(ModelState);
        }
    
        [HttpPatch] 
        [Route("AddEmails")]
        public async Task<IActionResult> IncludeEmails(MessageSessionAddUsersViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    try
                    {
                        var session = await sessionRepository.AddUsersToSessionAsync(viewModel.SessionId, fromEmail, viewModel.Values);
                        return Ok(session);
                    }
                    catch(EmailDoesNotMatchMessageSessionOwnerException)
                    {
                        return BadRequest("Only the session owner can add Users to a session.");
                    }
                }
            }
            return BadRequest(ModelState);
        }
    
        [HttpPatch]
        [Route("AddMessage")] 
        public async Task<IActionResult> AddMessageToSession(MessageSessionAddMessageViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    try
                    {
                        var session = await sessionRepository.AddMessageToSessionAsync(
                            viewModel.SessionId, fromEmail, 
                            new Message()
                            {
                                MessageSessionId = viewModel.SessionId,
                                Email = fromEmail,
                                Type = viewModel.Type,
                                Content = viewModel.Content, 
                            }
                        );

                        return Ok(session);
                    }
                    catch(EmailNotAssociatedWithMessageSessionException)
                    {
                        return BadRequest("User cannot add message to session they're not associated with");
                    }
                }
            }
            return BadRequest(ModelState);
        }
    }
}