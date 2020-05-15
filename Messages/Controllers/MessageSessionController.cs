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

        [HttpGet]
        [Route("GetSessions")]
        public async Task<IActionResult> GetUserSessions(AuthorizedJwtViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    return Ok(
                        sessionRepository.GetMessageSessions(fromEmail) 
                    );
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("CreateSession")]
        public async Task<IActionResult> CreateSession(MessageSessionCreateViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                // Check status of SECRET to ensure that emails are added
                // from a valid source
                if(viewModel.Secret != Shared.Services.Authorization.SECRET)
                {
                    return BadRequest("Secret not matched.");
                }

                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    var session = new MessageSession
                    {
                        Title = viewModel.Title,
                        Description = viewModel.Description,
                        Emails = fromEmail
                    };

                    if(viewModel.Emails != null)
                    {
                        session.Emails += ";" + string.Join(';', viewModel.Emails).ToLower();
                    }

                    await sessionRepository.CreateMessageSessionAsync(session);
                    return Ok(session);
                }

                return BadRequest("Specified Token Not Found");
            }
            return BadRequest(ModelState);
        }

        [HttpPost] 
        [Route("DeleteSession")]
        public async Task<IActionResult> DeleteSession(AuthorizedIntViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    try
                    {
                        var session = await sessionRepository.DeleteMessageSessionAsync(viewModel.Value, fromEmail);
                        if(session == null)
                        {
                            return BadRequest("Session does not exist.");
                        }
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

        [HttpGet]
        [Route("GetSessionUsers")]
        public async Task<IActionResult> GetSessionUsers(AuthorizedIntViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    try
                    {
                        return Ok(
                            await sessionRepository.GetSessionUsersAsync(viewModel.Value, fromEmail)
                        );
                    }
                    catch(EmailNotAssociatedWithMessageSessionException)
                    {
                        return BadRequest("User must be associated with session to get users");
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPatch] 
        [Route("AddUsersToSession")]
        public async Task<IActionResult> AddUsersToSession(MessageSessionAddRemoveUsersViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                // Check status of SECRET to ensure that emails are added
                // from a valid source
                if(viewModel.Secret != Shared.Services.Authorization.SECRET)
                {
                    return BadRequest("Secret not matched.");
                }

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
        [Route("RemoveUsersFromSession")]
        public async Task<IActionResult> RemoveUsersFromSession(MessageSessionAddRemoveUsersViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    try
                    {
                        return Ok (
                                await sessionRepository.RemoveUsersFromSessionAsync(
                                viewModel.SessionId, fromEmail, viewModel.Values
                            )
                        );
                    }
                    catch(EmailDoesNotMatchMessageSessionOwnerException)
                    {
                        return BadRequest("Only session owners can remove users from a session");
                    }
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPatch]
        [Route("AddMessageToSession")] 
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

        [HttpGet]
        [Route("GetMessagesFromSession")]
        public async Task<IActionResult> GetMessagesForSession(AuthorizedIntViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    try
                    {
                        return Ok(
                            await sessionRepository.GetMessagesForSessionAsync(
                                viewModel.Value,
                                fromEmail
                            )
                        );
                    }
                    catch(EmailNotAssociatedWithMessageSessionException)
                    {
                        return BadRequest("User must be associated with message session to see messages.");
                    }
                }
            }
            return BadRequest(ModelState);
        }        
    }
}