using MetadataExtractor;
using Microsoft.Extensions.FileProviders;
using PhotoAPI.Models;
using System.Drawing;
using System.Drawing.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace PhotoAPI.Helpers
{
    ///<Summary>
    /// Extension methods
    ///</Summary>
    public static class Helpers
    {
        /// <summary>
        /// Saving Image to the Volume by using this function
        /// </summary>
        /// <param name="imageFile"></param>
        /// <returns></returns>
        public static async Task<string> WriteImageToFolder(IFormFile imageFile)
        {

            string imageName = new string(Path.GetFileNameWithoutExtension(imageFile.Name).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), imageName);


            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return imagePath;
        }

        /// <summary>
        /// Convert Operation
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns>string</returns>
        public static async Task<string> ConvertFromBytesToBase64(string imagePath)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(imagePath);
                return Convert.ToBase64String(fileBytes);
            }
            catch (Exception)
            {
                return "There was an error while fetching this image";
            }

        }

        public static async Task<(bool isSuccess, string resizedbs64, string ErrorMessage)> ProcessImage(string imagePath, int size)
        {
            try
            {
  
                byte[] fileBytes = File.ReadAllBytes(imagePath);

                var (isSuccess, base64, errorMessage) = await ChangeImageSize(imagePath, size); // => This isnt the problem.

                if (isSuccess)
                {
                    return (true, base64, null);
                }
               
                return (false, null, errorMessage);
                
            }
            catch (Exception ex)
            {
                //In case if the file is not exists in the local storage.
                return (false, null, ex.Message);
            }

        }


        /// <summary>
        /// Uses 3rd part library to resize the image. Some system.drawing dll related problems occured in docker environment.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <param name="resize"></param>
        /// <returns></returns>
        public static async Task<(bool isSuccess, string base64, string errorMessage)> ChangeImageSize(string imagePath, int resize)
        {
            try
            {
                using var outStream = new MemoryStream();
                using SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(Path.Combine(imagePath));
                var clone = image.Clone(option => option
                 .Resize(new ResizeOptions
                 {
                     Mode = ResizeMode.Max,
                     Size = new SixLabors.ImageSharp.Size(resize, resize)
                 }));

                var res = clone.ToBase64String(JpegFormat.Instance);

                return (true, res.ToString(), null);
            }
            catch (Exception)
            {
                return (false, null, "Error while resizing the image");
            }
          
        }




        //Net 6 with Docker Environment doesnt support below methods

        //public static async Task<string> ConvertImageToBase64(System.Drawing.Image image)
        //{
        //    //Returning resized image as Base64 for future use.
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        image.Save(memoryStream, ImageFormat.Jpeg);
        //        byte[] imageBytes = memoryStream.ToArray();
        //        return Convert.ToBase64String(imageBytes);
        //    }
        //}

        //public static async Task<System.Drawing.Image> ConvertFromByteToImage(byte[] byteArr)
        //{
        //    using (var memorystrm = new MemoryStream(byteArr))
        //    {
        //        return System.Drawing.Image.FromStream(memorystrm);
        //    }
        //}


    }
}
