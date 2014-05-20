using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices;
using System.IO;

namespace Customer.Models
{
    public class Category
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(60)]
        [Display(Name="Menu Name")]
        public string MenuName { get; set; }

        /// <summary>
        /// usually the MenuName with spaces and special characters converted to dashes, but can
        /// be typed in by the user
        /// </summary>
        public string Url { get; set; }

        [Required]
        [Display(Name="Page Title")]
        public string PageTitle { get; set; }

        /// <summary>
        /// for page meta data description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// for page meta data key words
        /// </summary>
        [Display(Name="Key Words")]
        public string KeyWords { get; set; }

        // public or private
        [Display(Name="Keep this page private?")]
        public bool IsPrivate { get; set; }

        public Guid ContentID { get; set; }

    }


    public class Product
    {
        public int ID { get; set; }

        public int CategoryID { get; set; }

        [Required]
        [MaxLength(60)]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public Guid ImageFolder { get; set; }

        public string Tags { get; set; }

        public DateTime Created { get; set; }

        public bool IsPrivate { get; set; }

        /// <summary>
        /// Returns the first image for the product
        /// </summary>
        /// <returns></returns>
        public SiteImage FirstImage()
        {
            using (WebContext db = new WebContext())
            {
                SiteImage image = db.SiteImages.Where(i => i.ImageFolder == ImageFolder).OrderBy(s => s.Sequence).Take(1).FirstOrDefault();
                if (image == null)
                    return new SiteImage() { ID = 0, ImageFolder = Guid.Empty, Name = "Image Not Found", Sequence = 0 };
                else
                    return image;
            }
        }

        /// <summary>
        /// Returns all the images sorted by sequence
        /// </summary>
        /// <returns></returns>
        public List<SiteImage> AllImages()
        {
            using (WebContext db = new WebContext())
            {
                return db.SiteImages.Where(i => i.ImageFolder == ImageFolder).OrderBy(s => s.Sequence).ToList();
            }
        }

    }


    public class CategoryPage
    {
        public Category Category { get; set; }
        public List<Product> Products { get; set; }
    }


}