using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Images.Models;
using Images.Tools;
using Images.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.ViewModels;

namespace Images.Controllers
{
    [ApiController]
    [Route("/")]
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

        /// <summary>
        /// Creates all Directories for image uploading
        /// The root folder, and two folders in the root folder
        /// (Profiles and Messages)
        /// </summary>
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
        [DisableRequestSizeLimit]
        [Route("PostProfileImage")]
        public async Task<IActionResult> PostProfileImage()
        {
            CreateDirectories();      

            if(ModelState.IsValid)
            {
                var file = Request.Form.Files.First();

                var auth = Request.Form.First(f => f.Key == "Auth").Value;
                var viewModel = JsonConvert.DeserializeObject<AuthorizedJwtViewModel>(auth);

                var fromEmail = await
                    Services.AuthorizationServices.VerifyToken(clientFactory, viewModel.JwtFrom);

                if (fromEmail != null)
                {
                    var filePath = Path.Combine(ROOT_DIR, PROFILE_DIR, Path.GetFileName(file.FileName));
                    var photoPath = await photoRepo.GetPhotoPath(fromEmail);

                    // If there is already a PhotoPath that exists, delete
                    // it and the thumbnail
                    if (photoPath != null)
                    {
                        System.IO.File.Delete(photoPath.PhotoPath);
                        //System.IO.File.Delete(photoPath.PhotoPath.Replace(".", "THUMB."));
                        await photoRepo.UpdatePhotoPath(
                            fromEmail, filePath
                        );
                    }
                    else
                    {
                        await photoRepo.AddPhotoPath(new ProfilePhotoPath
                        {
                            Email = fromEmail,
                            PhotoPath = filePath
                        });
                    }

                    using (var photoStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(photoStream);
                    }

                    //var thumbImage = ImageTools.ConvertToSize(file.OpenReadStream(), 64, 64);
                    //thumbImage.Save(filePath.Replace(".", "THUMB.")); 

                    return Ok();
                } 
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("GetProfileImage")]
        public async Task<IActionResult> GetProfileImage(AuthorizedProfileImageViewModel viewModel)
        {
            CreateDirectories();
            if (ModelState.IsValid)
            {
                var photoPath = await photoRepo.GetPhotoPath(viewModel.Email);
                if (photoPath == null)
                {
                    return NotFound();
                }

                FileStream imageStream;
                //if (viewModel.IsThumb)
                    //imageStream = System.IO.File.OpenRead(photoPath.PhotoPath.Replace(".", "THUMB."));
                //else
                    imageStream = System.IO.File.OpenRead(photoPath.PhotoPath);

                var dotSplits = Path.GetFileName(photoPath.PhotoPath).Split('.');

                return File(imageStream, $"image/{dotSplits[dotSplits.Length - 1]}");
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
