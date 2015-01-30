using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Customer.Models;
using System.IO;



namespace Customer.Controllers
{
    public class CategoryController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }


        [Authorize]
        public ActionResult Edit(int ID)
        {
            using (WebContext db = new WebContext())
            {
                return View(db.Categories.FirstOrDefault(c => c.ID == ID));
            }
        }


        [Authorize]
        [HttpPost]
        public ActionResult Edit(Category model)
        {

            using (WebContext db = new WebContext())
            {
                db.Categories.Add(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", "Category", new { Id = model.ID });
            }
        }

        
        /// <summary>
        /// Returns an empty form to create a new category
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// Processes the request to create a new category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Create(Category model)
        {
            if (ModelState.IsValid)
            {
                using (WebContext db = new WebContext())
                {
                    // create new HtmlContent
                    HtmlContent content = new HtmlContent() { ID = Guid.NewGuid(), HTML = string.Format("<h2>{0}</h2>", model.MenuName) };
                    db.HtmlContent.Add(content);

                    // update the category with the html content id and save
                    model.ContentID = content.ID;
                    db.Categories.Add(model);

                    db.SaveChanges();

                    // load the new category page
                    return RedirectToAction("Details", "Category", new { Id = model.ID });
                }
            }
            else
            {
                ModelState.AddModelError("", "The Category could not be created, please fix the errors and try again");
            }

            return View(model);
        }
        

        [Authorize]
        public ActionResult Delete(int Id)
        {
            using (WebContext db = new WebContext())
            {
                Category category = db.Categories.Find(Id);

                foreach (Product product in db.Products.Where(p => p.CategoryID == category.ID))
                {
                    // remove each product
                    db.Products.Remove(product);

                    // delete the image records
                    foreach (SiteImage image in db.SiteImages.Where(i => i.ImageFolder == product.ImageFolder))
                        db.SiteImages.Remove(image);

                    // delete the images
                    try
                    {
                        Directory.Delete(Server.MapPath("~/Files/" + product.ImageFolder), true);
                    }
                    catch { }

                }

                // remove the category record
                db.Categories.Remove(category);

                // save database changes
                db.SaveChanges();

                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// Displays a category 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult Details(int Id)
        {
            using (WebContext db = new WebContext())
            {
                Category category = db.Categories.FirstOrDefault(c => c.ID == Id);
                List<Product> products = db.Products.Where(p => p.CategoryID == Id).ToList();
                CategoryPage model = new CategoryPage() { Category = category, Products = products };
                return View(model);
            }
        }

        
        public ActionResult CategoryPreviews()
        {
            List<CategoryPage> items = new List<CategoryPage>();

            using (WebContext db = new WebContext())
            {
                foreach (Category c in db.Categories)
                {
                    CategoryPage item = new CategoryPage() { Category = c, Products = db.Products.Where(p => p.CategoryID == c.ID).Take(1).ToList() };
                    items.Add(item);
                }
            }

            return PartialView(items);
        }


        public ActionResult MenuItems()
        {
            List<Category> items = new List<Category>();
            using (WebContext db = new WebContext())
            {

                string controller = Request.RequestContext.RouteData.Values["controller"].ToString();
                string action = Request.RequestContext.RouteData.Values["action"].ToString();

                if (controller == "Category" && action == "Details")
                    ViewBag.SelectedCategory = Request.RequestContext.RouteData.Values["Id"].ToString();

                if (controller == "Product" && action == "Details")
                {
                    int productId = Convert.ToInt32(Request.RequestContext.RouteData.Values["Id"].ToString());
                    Product product = db.Products.Find(productId);
                    ViewBag.SelectedCategory = product.CategoryID.ToString();
                }

                items = db.Categories.OrderBy(c=>c.Sequence).ToList();
            }
             return PartialView(items);
        }

        [Authorize]
        [HttpPost]
        public string Sort(string ids)
        {
            using (WebContext db = new WebContext())
            {
                int sort = 0;
                foreach (string id in ids.Split(','))
                {
                    Category category = db.Categories.Find(Convert.ToInt32(id));
                    category.Sequence = sort;

                    db.Entry(category).State = System.Data.Entity.EntityState.Modified;

                    sort += 1;
                }
                db.SaveChanges();
            }

            return "OK";
        }
    }
}
