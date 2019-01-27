using System.Web;
using System.Web.Helpers;

namespace WebAlbum.Web.Helpers
{
    public static class PhotoBuilder
    {
        public static byte[] CropImage(HttpPostedFileBase file)
        {
            var newImage = new WebImage(file.InputStream);

            var width = newImage.Width;
            var height = newImage.Height;

            if (width > height)
            {
                var leftRightCrop = (width - height) / 2;
                newImage.Crop(0, leftRightCrop, 0, leftRightCrop);
            }
            else if (height > width)
            {
                var topBottomCrop = (height - width) / 2;
                newImage.Crop(topBottomCrop, 0, topBottomCrop, 0);
            }

            newImage.Resize(250, 250, false, false);
            return newImage.GetBytes();
        }
    }
}