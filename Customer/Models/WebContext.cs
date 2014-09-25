using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Customer.Models
{
    public class WebContext: DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVideo> ProductVideos { get; set; }
        public DbSet<HtmlContent> HtmlContent { get; set; }
        public DbSet<ImageSetting> ImageSettings { get; set; }
        public DbSet<SiteImage> SiteImages { get; set; }
        public DbSet<AccountRegistrationCode> RegistrationCodes { get; set; }
        public DbSet<ContactForm> ContactForms { get; set; }
        public DbSet<MyAccount> MyAccount { get; set; }
    }
}