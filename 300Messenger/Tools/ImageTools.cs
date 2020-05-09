using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace _300Messenger.Tools
{
    public static class ImageTools
    {
        private static int EXIF_VALUE = 0x0112;

        /// <summary>
        /// Saves the file, after orienting it via EXIF property.
        /// Returns the string to the filename.
        /// </summary>
        public static string SaveAndOrientImage(IFormFile file, IWebHostEnvironment hostingEnvironment)
        {
            // ALWAYS use Path.GetFileName with file.FileName
            // Using file.FileName ONLY can result in malicious use of the site's directories
            var filename = Path.GetFileName(file.FileName);

            filename = $"{Guid.NewGuid().ToString()}_{filename}";
            string imageDirectory = Path.Combine(
                hostingEnvironment.WebRootPath, "Images",
                filename
            );

            using(var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                using(var img = Image.FromStream(memoryStream))
                {
                    if(img.PropertyIdList.Contains(EXIF_VALUE))
                    {
                        PropertyItem propOrientation = img.GetPropertyItem(EXIF_VALUE);
                        short val = BitConverter.ToInt16(propOrientation.Value, 0);
                        RotateFlipType rot = RotateFlipType.RotateNoneFlipNone;

                        if (val == 3 || val == 4)
                            rot = RotateFlipType.Rotate180FlipNone;
                        else if (val == 5 || val == 6)
                            rot = RotateFlipType.Rotate90FlipNone;
                        else if (val == 7 || val == 8)
                            rot = RotateFlipType.Rotate270FlipNone;

                        if (val == 2 || val == 4 || val == 5 || val == 7)
                            rot |= RotateFlipType.RotateNoneFlipX;

                        if (rot != RotateFlipType.RotateNoneFlipNone)
                            img.RotateFlip(rot);
                    }

                    img.Save(imageDirectory);
                }
            }    
                    
            return filename;
        }
    }
    public class FileUploadIncorrectType : Exception
    {
        public FileUploadIncorrectType(string msg) : base(msg) { }
    }

    public class FileUploadTooLargeException : Exception
    {
        public FileUploadTooLargeException(string msg) : base(msg) { }
    }
}