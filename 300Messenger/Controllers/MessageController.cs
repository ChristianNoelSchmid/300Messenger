using _300Messenger.Models;
using _300Messenger.Models.Repositories;
using _300Messenger.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace _300Messenger.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageRepository messageRepository;
        private readonly IWebHostEnvironment hostEnvironment; 

        public MessageController(IMessageRepository messageRepository, IWebHostEnvironment environment)
        {
            this.messageRepository = messageRepository;
            this.hostEnvironment = environment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateText(MessageSessionAddMessageViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                messageRepository.CreateMessage(
                    new Message() {
                        MessageSessionId = viewModel.Session.Id,
                        Type = viewModel.Type,
                        Content = viewModel.Content,
                    }
                );
            }

            return RedirectToAction("Details", "MessageSession", new { id = viewModel.Session.Id });
        }
    }
}