using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using _300Messenger.Images.Models;
using _300Messenger.Images.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _300Messenger.Images.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private const string ROOT_DIR = "Media";
        private const string PROFILE_DIR = "Profiles";
        private const string MESSAGE_DIR = "Messages";
        private readonly IProfilePhotoPathRepo photoRepo;
        private readonly IHttpClientFactory clientFactory;        
        private readonly ILogger<ImageController> _logger;

        public ImageController(IProfilePhotoPathRepo photoRepo, IHttpClientFactory clientFactory, ILogger<ImageController> logger)
        {
            this.photoRepo = photoRepo;
            this.clientFactory = clientFactory;
            this._logger = logger;
        }

        private void CreateDirectories()
        {
            if(!Directory.Exists(ROOT_DIR)) 
            { 
                Directory.CreateDirectory(ROOT_DIR); 
            }
            if(!Directory.Exists(Path.Combine(ROOT_DIR, PROFILE_DIR))) 
            { 
                Directory.CreateDirectory(Path.Combine(ROOT_DIR, PROFILE_DIR));
            }
            if(!Directory.Exists(Path.Combine(ROOT_DIR, MESSAGE_DIR)))
            {
                Directory.CreateDirectory(Path.Combine(ROOT_DIR, MESSAGE_DIR));
            }
        }

        [HttpPost]
        [Route("PostProfileImage")]
        public async Task<IActionResult> PostProfileImage(ProfileImageViewModel viewModel)
        {
            CreateDirectories();      

            if(ModelState.IsValid)
            {
                var fromEmail = await
                    Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    var filePath = Path.GetFileName(viewModel.FormFile.FileName);
                    if(photoRepo.GetPhotoPath(fromEmail) != null)
                    {
                        await photoRepo.RemovePhotoPath(fromEmail);

                        await photoRepo.AddPhotoPath(
                            new ProfilePhotoPath() {
                                Email = fromEmail,
                                PhotoPath = filePath
                            }
                        );
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                        await viewModel.FormFile.CopyToAsync(fileStream);
                    }

                    return Ok();
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("GetProfileImage")]
        public async Task<IActionResult> GetProfileImage(AuthorizedJwtViewModel viewModel)
        {
            CreateDirectories();      
            if(ModelState.IsValid)
            {
                var fromEmail = await
                    Shared.Services.Authorization.VerifyToken(clientFactory, viewModel.FromJwt);

                if(fromEmail != null)
                {
                    var image = System.IO.File.OpenRead(
                        await photoRepo.GetPhotoPath(fromEmail)
                    );
                    return File(image, "image/");
                }
            }

            return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("GetMessageImage")]
        public IActionResult GetMessageImage(string filename)
        {
            CreateDirectories();      

            var image = System.IO.File.OpenRead(
                Path.Combine(ROOT_DIR, MESSAGE_DIR, filename)
            );
            return File(image, "image/");
        }
    }
}
