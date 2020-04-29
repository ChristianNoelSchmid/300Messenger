using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _300Messenger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _300Messenger.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageRepository messageRepository;

        public MessageController(IMessageRepository repository)
        {
            this.messageRepository = repository;
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
        public ActionResult Create(Message message)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                messageRepository.CreateMessage(message);
                return RedirectToAction("details", new { id = message.Id });
            }
            catch
            {
                return View();
            }
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
        }

        // GET: Message/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Message/Delete/5
        [HttpPost]
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