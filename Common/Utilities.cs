using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using scribble.Models;
using System.Text;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace scribble.Common
{
    public static class Extensions
    {
        public static string CleanSQL(this String str)
        {
            return str.Replace("'", "''");
        }

    }

    public class Utilities
    {
        public static User GetLoggedInUser()
        {
            User u = new User();
            if (!String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                u = User.Get(int.Parse(HttpContext.Current.User.Identity.Name));
            }
            return u;
        }

        public static int GetLoggedInUserID()
        {
            int userid = 0;
            if (!String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                userid = int.Parse(HttpContext.Current.User.Identity.Name);
            }
            return userid;
        }
      
        public static bool IsAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }
        
        public static string HMACSH1(string signatureString)
        {
            var enc = Encoding.ASCII;
            HMACSHA1 hmac = new HMACSHA1(enc.GetBytes(System.Configuration.ConfigurationManager.AppSettings["Disqus_Secret"]));
            hmac.Initialize();

            byte[] buffer = enc.GetBytes(signatureString);
            return BitConverter.ToString(hmac.ComputeHash(buffer)).Replace("-", "").ToLower();
        }

        public static String ToBase64String(String source)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(source);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
            //return Convert.ToBase64String(Encoding.Unicode.GetBytes(source));
        }

        public static string GenerateDisqusSSOPayload(object payload)
        {
            var json = new JavaScriptSerializer().Serialize(payload);
            string timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString("00");
            string message = Utilities.ToBase64String(json);
            return (message + " " + Utilities.HMACSH1(message + " " + timestamp) + " " + timestamp);
        }

        //image utilities
        public static Stream GenerateThumb(Stream sourceFileStream, int scale, bool crop)
        {
            Image imgOriginal = Image.FromStream(sourceFileStream);
            Image imgActual = ScaleBySize(imgOriginal, scale);
            if (crop)
            {
                imgActual = CropImage(imgActual, new Rectangle(0, 0, scale, scale));
            }

            EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters();
            long[] quality = new long[1];

            quality[0] = 75L;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            encoderParams.Param[0] = encoderParam;

            MemoryStream ms = new MemoryStream();


            imgActual.Save(ms, encoders[1], encoderParams);
            imgActual.Dispose();
            imgOriginal.Dispose();
            return ms;
        }

        public static Image CropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            //reduce file size
            return (Image)(bmpCrop);
        }

        public static Image ScaleBySize(Image imgPhoto, int size)
        {
            float sourceWidth = imgPhoto.Width;
            float sourceHeight = imgPhoto.Height;
            float destHeight = 0;
            float destWidth = 0;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            // Resize Image to have the height = logoSize/2 or width = logoSize.
            // Height is greater than width, set Height = logoSize and resize width accordingly
            if (sourceWidth < sourceHeight)
            {
                destWidth = size;
                destHeight = (float)(sourceHeight * size / sourceWidth);

            }
            else
            {
                int h = size;
                destHeight = h;
                destWidth = (float)(sourceWidth * h / sourceHeight);
            }
            // Width is greater than height, set Width = logoSize and resize height accordingly

            //always force width to be the determining factor
            //destWidth = size;
            //destHeight = (float)(sourceHeight * size / sourceWidth);

            Bitmap bmPhoto = new Bitmap((int)destWidth, (int)destHeight, PixelFormat.Format32bppPArgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, (int)destWidth, (int)destHeight),
                new Rectangle(sourceX, sourceY, (int)sourceWidth, (int)sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();

            return bmPhoto;
        }

        public static void ResizeImage(string filename, string filepath)
        {
            //String fileExt = System.IO.Path.GetExtension(filename).ToLower();
            //string ThumbPath = String.Format("~/Scribbles/{0}_t_.{1}", filename, fileExt);
            //string MediumPath = String.Format("~/Scribbles/{0}_m_.{1}", filename, fileExt);
            //string OriginalPath = String.Format("~/Scribbles/{0}_o_.{1}", filename, fileExt);

            //using (var fileStream = File.Create(HttpContext.Current.Server.MapPath(ThumbPath)))
            //{
            //    GenerateThumb(imageStream, 200, false).CopyTo(fileStream);
            //}           

            using (System.Drawing.Image Img = System.Drawing.Image.FromFile(filepath + filename))
            {
                // Get file extension
                String fileExt = System.IO.Path.GetExtension(filepath + filename).ToLower();

                string ThumbPath = String.Format("{0}t_{1}", filepath, filename);
                string MediumPath = String.Format("{0}m_{1}", filepath, filename);

                // Get thumb resolution
                int thumnbnailSize = 200;
                int mediumSize = 400;

                using (Image ImgThnail = ScaleBySize(Img,thumnbnailSize))
                {
                    ImgThnail.Save(ThumbPath, Img.RawFormat);
                }
                using (Image ImgThnail = ScaleBySize(Img, mediumSize))
                {
                    ImgThnail.Save(MediumPath, Img.RawFormat);
                }
            }
        }

        public static Size NewImageSize(int OriginalHeight, int OriginalWidth, double thumbWidth, double thumbHeight)
        {
            Size NewSize;
            double tempval;
            if (OriginalHeight > thumbHeight || OriginalWidth > thumbWidth)
            {
                if (OriginalHeight > OriginalWidth)
                    tempval = thumbHeight / Convert.ToDouble(OriginalHeight);
                else
                    tempval = thumbWidth / Convert.ToDouble(OriginalWidth);

                NewSize = new Size(Convert.ToInt32(tempval * OriginalWidth),
                 Convert.ToInt32(tempval * OriginalHeight));
            }
            else
                NewSize = new Size(OriginalWidth, OriginalHeight); return NewSize;
        }
        
        //public static string[] HmacSha1Sign(byte[] keyBytes, string message)
        //{
        //    HMACSHA1 myhmacsha1 = new HMACSHA1(key);
        //    byte[] byteArray = Encoding.ASCII.GetBytes(input);
        //    MemoryStream stream = new MemoryStream(byteArray);
        //    return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
  

        //}

    }
}