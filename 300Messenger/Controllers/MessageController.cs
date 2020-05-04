using _300Messenger.Models;
using _300Messenger.Models.Repositories;
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

        public ActionResult Create(MessageSession session, )
    }
}