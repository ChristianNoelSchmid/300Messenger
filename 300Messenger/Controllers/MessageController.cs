using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _300Messenger.Models;
using _300Messenger.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _300Messenger.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageRepository messageRepository;

        // The IWebHostEnvironment interface retrieves information
        // about the hosting environment. It's from this interface that
        // we retrieve the directory to the wwwroot folder 
        // (see Create(MessageCreateViewModel) method)
        private readonly IWebHostEnvironment hostingEnvironment;

        public MessageController(IMessageRepository repository,
                                 IWebHostEnvironment hostingEnvironment)
        {
            this.messageRepository = repository;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Message
        public ActionResult Index()
        {
            return View(messageRepository.GetMessages());
        }

        // GET: Message/Details/5
        public ActionResult Details(int id)
        {
            var message = messageRepository.GetMessage(id);
            return View(message);
        }

        // GET: Message/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Message/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MessageCreateViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                StringBuilder uniqueFilenames = new StringBuilder();
                if(viewModel.Image != null) 
                {
                    uniqueFilenames.Append(ImageTools.SaveAndOrientImage(viewModel.Image, hostingEnvironment) + ",");
                    uniqueFilenames.Remove(uniqueFilenames.Length - 1, 1);
                }
                
                var message = messageRepository.CreateMessage(new Message() {
                    Email = viewModel.Email,
                    Content = viewModel.Content,
                    ImagePaths = uniqueFilenames.Length > 0 ? uniqueFilenames.ToString() : null
                });
                return RedirectToAction("details", new { id = message.Id });
            }

            return View(viewModel);
        }

        /*// GET: Message/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Message/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/

        public IActionResult Delete(int id)
        {
            messageRepository.DeleteMessage(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: Message/Delete/5
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/
    }
}