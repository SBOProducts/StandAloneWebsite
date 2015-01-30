using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Customer.Helpers;
using Customer.Models;
using System.IO;

namespace Customer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Service()
        {
            return View();
        }
        /*
        public ActionResult BrowerImages()
        {

            Importer importer = new Importer();
            using (WebContext db = new WebContext())
            {

                // download images
                List<ImageSetting> imageSettings = db.ImageSettings.Where(s => s.Key == "ProductImages").ToList();
                List<ImportedImage> images = importer.GetImages();
                
                foreach (ImportedProduct p in importer.GetProducts())
                {
                    Product realProduct = db.Products.Where(c => c.Name == p.Title).FirstOrDefault();
                    string imgFolder = Server.MapPath(string.Format("~/Files/{0}/", realProduct.ImageFolder));

                    // go thorugh each of the images for this product from imported data
                    int sequence = 0;
                    ImportedImage[] prodImages = images.Where(i => i.ProductId == p.ProductId).ToArray();
                    foreach (ImportedImage img in prodImages)
                    {
                        ImageManager iMan;
                        // load the full size image from disk or url
                        string fullSizePath = Path.Combine(imgFolder, img.FileName);
                        if (System.IO.File.Exists(fullSizePath))
                        {
                            iMan = new ImageManager(fullSizePath);
                        }
                        else
                        {
                            iMan = new ImageManager(new Uri(img.Url));
                            iMan.Save(fullSizePath);
                        }
                        
                        // generate sizes
                        foreach (ImageSetting setting in imageSettings)
                        {
                            string scaledPath = string.Format("{0}{1}-{2}{3}", imgFolder, Path.GetFileNameWithoutExtension(img.FileName), setting.MaxSize, Path.GetExtension(img.FileName));
                            if (!System.IO.File.Exists(scaledPath))
                            {
                                iMan.ScaleImage(setting.MaxSize, setting.MakeSquareImage);
                                iMan.Save(scaledPath);
                                iMan.RestoreOriginalImage();
                            }
                        }

                        // add database record
                        SiteImage newImage = new SiteImage() { ImageFolder = realProduct.ImageFolder, Name = img.FileName, Sequence = sequence };
                        db.SiteImages.Add(newImage);
                        sequence += 1;
                    }

                    // update the database with new images
                    db.SaveChanges();
                    sequence = 0;
                }
            }

            return View();
        }

        public ActionResult BrowerImport()
        {

            using (WebContext db = new WebContext())
            {

                Importer importer = new Importer();
                
                string[] categories = (from p in importer.GetProducts() select p.Category).Distinct().ToArray();


                // create the categories
                foreach (string cat in categories)
                {
                    Category category = new Category() { ContentID = Guid.NewGuid(), MenuName = cat, PageTitle = cat };
                    db.Categories.Add(category);
                }
                db.SaveChanges();


                // add the products
                foreach (ImportedProduct p in importer.GetProducts())
                {
                    Product product = new Product()
                    {
                        CategoryID = db.Categories.Where(c => c.PageTitle == p.Category).First().ID,
                        Created = DateTime.Now,
                        Description = p.Description,
                        ImageFolder = Guid.NewGuid(),
                        Name = p.Title,
                        Price = p.Price
                    };
                    Directory.CreateDirectory(Server.MapPath("~/Files/" + product.ImageFolder));
                    db.Products.Add(product);
                }
                db.SaveChanges();


                

            }

            

            ViewBag.Message = "Completed";
            return View();
        }

        public ActionResult BrowerFix()
        {
            using (WebContext db = new WebContext())
            {
                foreach (Category cat in db.Categories)
                {
                    if (cat.ContentID == Guid.Empty)
                    {
                        cat.ContentID = Guid.NewGuid();
                        db.Entry(cat).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();
                }

                Category[] model = db.Categories.ToArray();
                return View(model);
            }
        }
        */
    }
}
