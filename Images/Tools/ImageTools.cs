using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Images.Tools
{
    public static class ImageTools
    {
        public static Image ConvertToSize(Stream imgStream, int width, int height)
        {
            try
            {
                Image image = Image.FromStream(imgStream);
                Image thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);

                return thumb;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
