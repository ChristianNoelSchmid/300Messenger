using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Messages.Exceptions;
using Messages.Models;
using Messages.Models.Repositories;
using Messages.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.Models;
using Shared.ViewModels;

namespace Messages.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageSessionController : Controller
    {
        private readonly IMessageSessionRepository sessionRepository;
        private readonly IHttpClientFactory clientFactory;

        public MessageSessionController(IMessageSessionRepository sessionRepository,
                                        IHttpClientFactory clientFactory)
        {
            this.sessionRepository = sessionRepository;
            this.clientFactory = clientFactory;
        }

        [HttpGet]
        [Route("GetSession")]
        public async Task<IActionResult> GetSession(AuthorizedIntViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Services.AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

                if(fromEmail != null)
                {
                    try
                    {
                        var session = await sessionRepository.GetMessageSessionAsync(viewModel.Value, fromEmail); 
                        return Ok(JsonConvert.SerializeObject(session));
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
                    await Services.AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

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
                var fromEmail = 
                    await Services.AuthorizationServices.VerifyToken(clientFactory, viewModel.FromJwt);

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
                    return Ok(JsonConvert.SerializeObject(session));
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
                    await Services.AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

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
            return BadRequest();
        }

        [HttpGet]
        [Route("GetSessionUsers")]
        public async Task<IActionResult> GetSessionUsers(AuthorizedIntViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Services.AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

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
 
        [HttpPost]
        [Route("AddMessage")] 
        public async Task<IActionResult> AddMessageToSession(AuthorizedMessageViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Services.AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

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

                        return Ok(JsonConvert.SerializeObject(session));
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
        [Route("GetMessages")]
        public async Task<IActionResult> GetMessagesForSession(AuthorizedIntViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var fromEmail = 
                    await Services.AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

                if(fromEmail != null)
                {
                    try
                    {
                        return Ok(
                            JsonConvert.SerializeObject(await sessionRepository.GetMessagesForSessionAsync(
                                viewModel.Value,
                                fromEmail
                            ))
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