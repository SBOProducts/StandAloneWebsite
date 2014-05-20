using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Customer.Models;
using System.IO;

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
                
                db.SaveChanges();

                // delete the images
                Directory.Delete(Server.MapPath("~/Files/" + model.ImageFolder), true);

                return RedirectToAction("Details", "Category", new { Id = model.CategoryID });
            }
        }


        public ActionResult Create(int category)
        {
            Product model = new Product() { CategoryID = category };
            return View(model);
        }


        [HttpPost]
        public ActionResult Create(Product model)
        {
            // the image key is the ImageFolder if images are uploaded before the product is created
            string imgkey = Request["imgkey"];

            if(ModelState.IsValid){
                model.Created = DateTime.Now;
                model.ImageFolder = new Guid(imgkey);
                using (WebContext db = new WebContext())
                {
                    db.Products.Add(model);
                    db.SaveChanges();

                    // create the image folder
                    Directory.CreateDirectory(Server.MapPath("~/Files/" + model.ImageFolder));

                    // take them to the list of products for the category
                    return RedirectToAction("Details", "Category", new { Id = model.CategoryID });
                }
            }

            return View(model);
        }


        public ActionResult Edit(int Id)
        {
            using (WebContext db = new WebContext())
            {
                return View(db.Products.FirstOrDefault(c => c.ID == Id));
            }
        }


        [HttpPost]
        public ActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                using (WebContext db = new WebContext())
                {
                    db.Products.Add(model);
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Details", "Product", new { Id = model.ID });
                }
            }

            return View(model);
        }


        public ActionResult Details(int Id)
        {
                using (WebContext db = new WebContext())
                {
                    return View(db.Products.FirstOrDefault(c => c.ID == Id));
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
