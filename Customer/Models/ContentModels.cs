using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Customer.Models
{

    public class HtmlContent
    {
        public Guid ID { get; set; }

        public Guid ImageFolder { get; set; }

        [DataType(DataType.Html)]
        public string HTML { get; set; }
    }


    public class SiteImage
    {
        public int ID { get; set; }

        public Guid ImageFolder { get; set; }

        public int Sequence { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Gets the virtual path to the file
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string GetVirtualPath(int size)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(Name);
            string ext = System.IO.Path.GetExtension(Name);
            string vPath = string.Format("~/Files/{0}/{1}-{2}{3}", ImageFolder, name, size, ext);
            string fullPath = HttpContext.Current.Server.MapPath(vPath);
            if (!System.IO.File.Exists(fullPath))
                return string.Format("~/Files/ImageNotFound-{0}.jpg", size);
            else
                return vPath;
        }
    }

    public class ImageUploadModel
    {
        /// <summary>
        /// the image folder where the uploaded files will be saved
        /// </summary>
        public Guid ImageFolder { get; set; }

        /// <summary>
        /// The key to retrieving image settings
        /// </summary>
        public string ImageSettings { get; set; }

        /// <summary>
        /// Returns a list of the images in the image folder
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<SiteImage> Images()
        {
            using (WebContext db = new WebContext())
            {
                return db.SiteImages.Where(i => i.ImageFolder == ImageFolder).OrderBy(i=>i.Sequence).ToList();
            }
        }
    }



    public class ContactForm
    {
        public int Id { get; set; }

        public DateTime Sent { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Required]
        public string From { get; set; }

        [Required]
        public string Message { get; set; }

        
    }


    public class ImageSetting
    {
        /// <summary>
        /// A unique id for the image setting
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Used to group settings by key name (eg. ProductImages)
        /// </summary>
        public string Key { get; set; }

        #region Watermark Settings

        /// <summary>
        /// Whether or not to display a watermark
        /// </summary>
        public bool ShowWatermark { get; set; }

        /// <summary>
        /// The text or image path to use as a watermark if enabled
        /// </summary>
        public string Watermark { get; set; }

        /// <summary>
        /// Determines how transparent the watermark symbol displays
        /// </summary>
        public int WatermarkOpacity { get; set; }

        /// <summary>
        /// Left, Middle, Right
        /// </summary>
        public string WatermarkHorizontalPosition { get; set; }

        /// <summary>
        /// Top, Middle, Bottom
        /// </summary>
        public string WatermarkVerticalPosition { get; set; }

        #endregion

        /// <summary>
        /// The maximum dimension for an image width or height, whichever is larger
        /// </summary>
        public int MaxSize { get; set; }

        /// <summary>
        /// Produces a square image using the Background color to fill the empty space
        /// </summary>
        public bool MakeSquareImage { get; set; }

        /// <summary>
        /// The color used to fill in the missing area when an image is transformed to a square
        /// </summary>
        public string BackgroundColor { get; set; }




    }


    public class OrbitSettings
    {
        public Guid ImageFolder { get; set; }
        
        public int Size { get; set; }

        public List<SiteImage> Images()
        {
            using (WebContext db = new WebContext())
            {
                return db.SiteImages.Where(i => i.ImageFolder == ImageFolder).OrderBy(i=>i.Sequence).ToList();
            }
        }
    }


}