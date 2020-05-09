using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _300Messenger.Models;
using _300Messenger.Models.Repositories;
using _300Messenger.Tools;
using _300Messenger.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _300Messenger.Controllers
{
    public class MessageSessionController : Controller
    {
        private readonly IMessageSessionRepository sessionRepository;

        // The IWebHostEnvironment interface retrieves information
        // about the hosting environment. It's from this interface that
        // we retrieve the directory to the wwwroot folder 
        // (see Create(MessageCreateViewModel) method)
        private readonly IWebHostEnvironment hostingEnvironment;

        public MessageSessionController(IMessageSessionRepository sessionRepository,
                                 IWebHostEnvironment hostingEnvironment)
        {
            this.sessionRepository = sessionRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Message
        public ActionResult Index()
        {
            return View(sessionRepository.GetMessageSessions());
        }

        // GET: Message/Details/5
        public ActionResult Details(int id)
        {
            var session = sessionRepository.GetMessageSession(id);

            if(session == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }

            var sessionViewModel = new MessageSessionAddMessageViewModel() 
            {
                Session = session,
                Type = MessageType.Image
            };
            return View(sessionViewModel);
        }



        // GET: Message/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Message/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MessageSessionCreateViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var session = sessionRepository.CreateMessageSession(
                    new MessageSession() {
                        Email = viewModel.Email,
                        Title = viewModel.Title,
                        Description = viewModel.Description,
                    }
                );
                return RedirectToAction("Details", "MessageSession", new { id = session.Id });
            }

            return View(viewModel);
        }

        // GET: Message/Edit/5
        public ActionResult Edit(int id)
        {
            var session = sessionRepository.GetMessageSession(id);
            var viewModel = new MessageSessionEditViewModel()
            {
                Id = session.Id,
                Title = session.Title,
                Description = session.Description,
                Email = session.Email
            };
            return View(viewModel);
        }

        // POST: Message/Edit/5
        [ValidateAntiForgeryToken]
        [HttpPatch]
        public ActionResult Edit(MessageSessionEditViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var session = sessionRepository.GetMessageSession(viewModel.Id); 
                session.Title = viewModel.Title;
                session.Description = viewModel.Description;
                session.Email = viewModel.Email;

                sessionRepository.UpdateMessageSession(session);

                return RedirectToAction("Details", new { id = viewModel.Id });
            }
            return View();
        }

        public IActionResult Delete(int id)
        {
            sessionRepository.DeleteMessageSession(id);
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