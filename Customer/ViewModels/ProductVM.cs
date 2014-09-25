using Customer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Customer.ViewModels
{
    public class ProductVM
    {
        public ProductVM()
        {
        }

        public ProductVM(Product data, List<ProductVideo> videos) : this(data)
        {
            if (videos.Count > 0)
                Video1 = videos[0].YouTubeUrl;
            if (videos.Count > 1)
                Video2 = videos[1].YouTubeUrl;
            if (videos.Count > 2)
                Video3 = videos[2].YouTubeUrl;
        }

        public ProductVM(Product data)
        {
            this.ID = data.ID;
            this.CategoryID = data.CategoryID;
            this.Created = data.Created;
            this.Description = data.Description;
            this.ImageFolder = data.ImageFolder;
            this.IsPrivate = data.IsPrivate;
            this.Name = data.Name;
            this.Price = data.Price;
            this.Tags = data.Tags;
        }

        public Product GetProduct()
        {
            return new Product()
            {
                CategoryID = this.CategoryID,
                Created = this.Created,
                Tags = this.Tags,
                Price = this.Price,
                Name = this.Name,
                IsPrivate = this.IsPrivate,
                ImageFolder = this.ImageFolder,
                Description = this.Description,
                ID = this.ID
            };
        }

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

        [AllowHtml]
        [Display(Name="Video 1")]
        public string Video1 { get; set; }

        [AllowHtml]
        [Display(Name = "Video 2")]
        public string Video2 { get; set; }

        [AllowHtml]
        [Display(Name = "Video 3")]
        public string Video3 { get; set; }

        public List<string> GetYouTubeUrls
        {
            get
            {
                List<string> urls = new List<string>();
                if (!string.IsNullOrEmpty(Video1))
                    urls.Add(Video1);
                if (!string.IsNullOrEmpty(Video2))
                    urls.Add(Video2);
                if (!string.IsNullOrEmpty(Video3))
                    urls.Add(Video3);
                return urls;
            }
        }

        public bool HasVideos { get { return GetYouTubeUrls.Count > 0; } }
    }
}