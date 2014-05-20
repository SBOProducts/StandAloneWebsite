using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;

namespace Customer.Models
{
    public class MyAccount
    {
        private static MyAccount _settings = null;

        public MyAccount()
        {
        }

        /// <summary>
        /// Gets a singleton instance of site settings for performance reasons
        /// </summary>
        public static MyAccount Instance
        {
            get
            {
                if (_settings == null)
                {
                    // try to load settings from the database
                    using (WebContext db = new WebContext())
                    {
                        _settings = db.MyAccount.FirstOrDefault();

                        if (_settings == null)
                        {
                            // settings not found in the database
                            // default site settings
                            _settings = new MyAccount() { BusinessName = "Company Name", EmailAddress = "myaddress@email.com", LogoImage = "~/Content/Images/Logos/DefaultLogo.png", SiteScripts = "<script type=\"text/javascript\">\n\n/* custom scripts here */\n\n</script>", SiteStyle="<style type=\"text/css\">\n\n/*Custom styles here */\n\n</style>" };
                            db.MyAccount.Add(_settings);
                            db.SaveChanges();
                        }
                    }
                }

                return _settings;
            }
        }


        /// <summary>
        /// Saves settings to the database
        /// </summary>
        public void Save()
        {
            using (WebContext db = new WebContext())
            {
                db.Entry(this).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }


        public Guid ID { get; set; }


        /// <summary>
        /// The name of the business
        /// </summary>
        [DisplayName("Business Name")]
        [Required]
        public string BusinessName { get; set; }


        /// <summary>
        /// Email from contact form will be sent here
        /// </summary>
        [DisplayName("Email Address")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Used to include custom site logic
        /// </summary>
        [DisplayName("Custom Javascript")]
        public string SiteScripts { get; set; }


        /// <summary>
        /// Used to modify site style
        /// </summary>
        [DisplayName("Custom CSS")]
        public string SiteStyle { get; set; }


        /// <summary>
        /// The currently used logo image
        /// </summary>
        public string LogoImage { get; set; }

        

        /// <summary>
        /// A list of available logo images in the logos directory
        /// </summary>
        [NotMapped]
        public List<string> AvailableLogoUrls
        {
            get
            {
                List<string> logos = new List<string>();
                foreach (string path in Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Content/Images/Logos")))
                {
                    logos.Add(string.Format("~/Content/Images/Logos/{0}", Path.GetFileName(path)));
                }
                return logos;
            }
        }

    }
}