using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Customer.Models;
using System.IO;
using Customer.ViewModels;

namespace Customer.Controllers
{
    public class ProductController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Delete the product and image related records
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Delete(int Id)
        {
            using (WebContext db = new WebContext())
            {
                // delete the data
                Product model = db.Products.FirstOrDefault(c => c.ID == Id);
                db.Products.Remove(model);

                // delete the image records
                foreach (SiteImage image in db.SiteImages.Where(i => i.ImageFolder == model.ImageFolder))
                    db.SiteImages.Remove(image);

                // delete video links
                foreach (ProductVideo video in db.ProductVideos.Where(v => v.ProductID == Id))
                    db.ProductVideos.Remove(video);
                
                db.SaveChanges();

                // delete the images
                Directory.Delete(Server.MapPath("~/Files/" + model.ImageFolder), true);

                return RedirectToAction("Details", "Category", new { Id = model.CategoryID });
            }
        }


        public ActionResult Create(int category)
        {
            Product product = new Product() { CategoryID = category };
            ProductVM model = new ProductVM(product);
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(ProductVM model)
        {
            // the image key is the ImageFolder if images are uploaded before the product is created
            string imgkey = Request["imgkey"];

            if(ModelState.IsValid){
                model.Created = DateTime.Now;
                model.ImageFolder = new Guid(imgkey);
                using (WebContext db = new WebContext())
                {
                    Product newProduct = model.GetProduct();
                    db.Products.Add(newProduct);
                    db.SaveChanges();
                    model.ID = newProduct.ID;

                    // create the image folder
                    Directory.CreateDirectory(Server.MapPath("~/Files/" + model.ImageFolder));

                    // attach videos
                    AddVideos(model);

                    // take them to the list of products for the category
                    return RedirectToAction("Details", "Category", new { Id = model.CategoryID });
                }
            }

            return View(model);
        }

        /// <summary>
        /// Adds youtube videos to products if any were provided
        /// </summary>
        /// <param name="model"></param>
        private void AddVideos(ProductVM model)
        {
            using (WebContext db = new WebContext())
            {
                foreach (string url in model.GetYouTubeUrls)
                {
                    ProductVideo video = new ProductVideo() { ProductID = model.ID, YouTubeUrl = url };
                    db.ProductVideos.Add(video);
                }
                db.SaveChanges();
            }
        }


        public ActionResult Edit(int Id)
        {
            using (WebContext db = new WebContext())
            {
                Product product = db.Products.FirstOrDefault(c => c.ID == Id);
                List<ProductVideo> videos = db.ProductVideos.Where(v => v.ProductID == Id).ToList();
                ProductVM model = new ProductVM(product, videos);
                return View(model);
            }
        }


        [HttpPost]
        public ActionResult Edit(ProductVM model)
        {
            if (ModelState.IsValid)
            {
                using (WebContext db = new WebContext())
                {
                    Product product = model.GetProduct();
                    db.Products.Add(product);

                    // delete out the videos and add them back in
                    db.ProductVideos.RemoveRange(db.ProductVideos.Where(v => v.ProductID == model.ID));

                    db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    AddVideos(model);

                    return RedirectToAction("Details", "Product", new { Id = model.ID });
                }

                
            }

            return View(model);
        }


        public ActionResult Details(int Id)
        {
            using (WebContext db = new WebContext())
            {
                Product product = db.Products.FirstOrDefault(c => c.ID == Id);
                List<ProductVideo> videos = db.ProductVideos.Where(v => v.ProductID == Id).ToList();
                ProductVM model = new ProductVM(product, videos);
                return View(model);
            }
        }


        public ActionResult Grid(IEnumerable<Product> products)
        {
            return PartialView(products);
        }


        public ActionResult List(IEnumerable<Product> products)
        {
            return PartialView(products);
        }
    }
}
