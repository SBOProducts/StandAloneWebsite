using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Customer.Models;
using System.IO;
using Customer.Helpers;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Xml;

namespace Customer.Controllers
{
    public class ContentController : Controller
    {

        public ActionResult ImageNavigator(Guid ImageFolder)
        {
            using (WebContext db = new WebContext())
            {
                List<SiteImage> images = db.SiteImages.Where(i => i.ImageFolder == ImageFolder).ToList();
                return PartialView(images);
            }
        }

        
        public ActionResult Index(Guid ID, string ContentType)
        {

            using (WebContext db = new WebContext())
            {
                // lookup the content
                HtmlContent model = db.HtmlContent.Find(ID);

                // if it doesn't exist then create new content
                if (model == null)
                {
                    //model = new HtmlContent() { ID = ID, HTML = "<h2>New Page</h2>\n<p><a href=\"/Help/ModifyContent\">Click here</a> to learn how to add content to your page.</p>" };
                    model = new HtmlContent() { ID = ID, HTML = GetDefaultContent(ContentType) };
                    db.HtmlContent.Add(model);
                    db.SaveChanges();
                     
                }

                // return the content
                return PartialView(model);
            }
        }


        public ActionResult Display(Guid ID, string ContentType)
        {

            using (WebContext db = new WebContext())
            {
                // lookup the content
                HtmlContent model = db.HtmlContent.Find(ID);

                // if it doesn't exist then create new content
                if (model == null)
                {
                    //model = new HtmlContent() { ID = ID, HTML = "<h2>New Page</h2>\n<p><a href=\"/Help/ModifyContent\">Click here</a> to learn how to add content to your page.</p>" };
                    model = new HtmlContent() { ID = ID, HTML = GetDefaultContent(ContentType) };
                    db.HtmlContent.Add(model);
                    db.SaveChanges();
                }

                // return the content
                return PartialView(model);
            }
        }

        /// <summary>
        /// Gets the default content for a new Html Content control
        /// </summary>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        private string GetDefaultContent(string ContentType)
        {
            if (string.IsNullOrEmpty(ContentType))
                ContentType = "default";

            ContentType = ContentType.ToLower();

            XmlDocument doc = new XmlDocument();

            doc.Load(Server.MapPath("~/DefaultContent.xml"));

            string xpath = string.Format("/content/{0}", ContentType);
            
            XmlNode node = doc.DocumentElement.SelectSingleNode(xpath);

            return node.InnerXml.ToString();
        }



        #region Html5 Upload Files

        public ActionResult ImageManagerPanel(ImageUploadModel model)
        {
            return PartialView(model);
        }


        public ViewResult UploadFiles(int Id)
        {
            return View(Id);
        }

        /// <summary>
        /// Delete a logo
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteLogo(string src)
        {
            try
            {
                string fileName = Path.GetFileName(src);
                string path = Server.MapPath(string.Format("~/Content/Images/Logos/{0}", fileName));
                System.IO.File.Delete(path);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }

        [HttpPost]
        public ActionResult UseLogo(string src)
        {
            MyAccount info = MyAccount.Instance;
            info.LogoImage = string.Format("~/Content/Images/Logos/{0}", Path.GetFileName(src));
            info.Save();
            return Json("OK");
        }

        [HttpPost]
        public ActionResult UploadLogo(string Id, string ImageSettings, IEnumerable<HttpPostedFileBase> files)
        {
            List<string> urls = new List<string>();

            foreach (HttpPostedFileBase file in files)
            {
                string path = Server.MapPath(string.Format("~/Content/Images/Logos/{0}", file.FileName));
                file.SaveAs(path);
                urls.Add(string.Format("/Content/Images/Logos/{0}", file.FileName));
            }

            return Json(urls);
        }

        [HttpPost]
        public ActionResult UploadFiles(string Id, string ImageSettings, IEnumerable<HttpPostedFileBase> files)
        {
            Guid ImageFolder = new Guid(Id);

            // ensure folder exists
            string folderPath = Server.MapPath(string.Format("~/Files/{0}/", ImageFolder));
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // lookup image settings
            List<ImageSetting> imageSettings = new List<ImageSetting>();
            using (WebContext db = new WebContext())
            {
                imageSettings = db.ImageSettings.Where(s => s.Key == ImageSettings).ToList();
            }

            List<string> urls = new List<string>();

            foreach (HttpPostedFileBase file in files)
            {
                ImageManager iMan = new ImageManager(file.InputStream);

                // generate sizes based on configuration
                foreach (ImageSetting setting in imageSettings)
                {
                    iMan.ScaleImage(setting.MaxSize, setting.MakeSquareImage);
                    string physicalPath = string.Format("{0}{1}-{2}{3}", folderPath, Path.GetFileNameWithoutExtension(file.FileName), setting.MaxSize, Path.GetExtension(file.FileName));
                    iMan.Save(physicalPath);
                    iMan.RestoreOriginalImage();

                    string url = string.Format("/Files/{0}/{1}-{2}{3}", ImageFolder, Path.GetFileNameWithoutExtension(file.FileName), setting.MaxSize, Path.GetExtension(file.FileName));
                    urls.Add(url);
                }

                // add image to db
                using (WebContext db = new WebContext())
                {
                    int seq = 0;
                    SiteImage lastImage = db.SiteImages.Where(i => i.ImageFolder == ImageFolder).OrderByDescending(i => i.Sequence).Take(1).FirstOrDefault();
                    if (lastImage != null)
                        seq = lastImage.Sequence + 1;

                    SiteImage image = new SiteImage() { ImageFolder = ImageFolder, Name = file.FileName, Sequence = seq + 1 };
                    db.SiteImages.Add(image);
                    db.SaveChanges();
                }
            }


            // clear the cache
            return Json(urls.ToArray());
        }


        public ActionResult DeleteFiles(Guid Id, int[] ids)
        {
            try
            {
                using (WebContext db = new WebContext())
                {
                    // loop through each image sent for deletion
                    foreach (int id in ids)
                    {
                        SiteImage image = db.SiteImages.Find(id);
                        db.Entry(image).State = System.Data.Entity.EntityState.Deleted;

                        string folder = Server.MapPath(string.Format("~/Files/{0}", Id));
                        string name = Path.GetFileNameWithoutExtension(image.Name);

                        foreach (string path in Directory.GetFiles(folder, name + "*"))
                            System.IO.File.Delete(path);

                        db.SaveChanges();
                    }

                    return Json("OK");
                }

            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }


        /// <summary>
        /// Sorts the images in the folder
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public string SortFiles(Guid Id, int[] ids)
        {
            try
            {
                using (WebContext db = new WebContext())
                {
                    foreach (SiteImage image in db.SiteImages.Where(i => i.ImageFolder == Id))
                    {
                        image.Sequence = Array.IndexOf(ids, image.ID);
                        db.Entry(image).State = System.Data.Entity.EntityState.Modified;
                    }

                    db.SaveChanges();
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return "Errored: " + ex.Message;
            }
        }

        #endregion


        public ActionResult Edit(Guid ID)
        {
            using (WebContext db = new WebContext())
            {
                // lookup the content
                HtmlContent model = db.HtmlContent.Find(ID);

                // return the content
                return PartialView(model);
            }
        }


        public ActionResult Orbit(OrbitSettings model)
        {
            return PartialView(model);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(HtmlContent model)
        {
            using (WebContext db = new WebContext())
            {
                db.HtmlContent.Add(model);
                if (db.HtmlContent.Count(m => m.ID == model.ID) == 1)
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                else
                    db.Entry(model).State = System.Data.Entity.EntityState.Added;

                db.SaveChanges();

                // clear deleted files
                string path = Server.MapPath("~/Files/" + model.ID);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                foreach (string file in Directory.GetFiles(path))
                {
                    string fileName = Path.GetFileName(file);
                    if (!model.HTML.Contains(fileName))
                        System.IO.File.Delete(file);
                }


                return RedirectToAction("Index", "Content", new { ID = model.ID });
            }
        }


        public ActionResult ContactForm()
        {
            ContactForm model = new ContactForm();
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult ContactForm(ContactForm model)
        {
            if (ModelState.IsValid)
            {

                // Save the record to database
                using (WebContext db = new WebContext())
                {
                    model.Sent = DateTime.Now;
                    db.ContactForms.Add(model);
                    db.SaveChanges();
                }

                try
                {
                    // try to send the message via email
                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = ConfigurationManager.AppSettings["SMTPHost"];

                    MailMessage message = new MailMessage();

                    message.Subject = "A message from your website contact form";

                    message.From = new MailAddress(model.EmailAddress, model.From);

                    message.To.Add(new MailAddress(ConfigurationManager.AppSettings["EmailAddress"]));
                    //message.To.Add(new MailAddress(MyAccount.Instance.EmailAddress));
                    message.IsBodyHtml = false;

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("The following message was sent using the contact form on your website");
                    sb.AppendLine();
                    sb.AppendLine(model.Message);

                    message.Body = sb.ToString();

                    smtpClient.Send(message);

                    @ViewBag.Message = "<div id=\"ContactFormMessage\" class=\"alert-box success\">Your message has been sent! Thank you.</div>";
                }
                catch (Exception ex)
                {
                    @ViewBag.Message = "<div id=\"ContactFormMessage\" class=\"alert-box alert\">We're sorry, there was a problem sending your message.</div><p>" + ex.Message + "</p>";
                }

                return PartialView();
                
            }

            return PartialView(model);

        }
    }
}
