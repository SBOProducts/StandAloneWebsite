using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using Customer.Models;
using System.IO;

namespace Customer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_EndRequest() 
        {

        }



        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            System.Data.Entity.Database.SetInitializer(new MyDataContextDbInitializer());

            /*
            // [2/6/14] Temp code to remove huge images
            using (WebContext db = new WebContext())
            {
                // delete the 1000px image from settings to prevent future creation
                ImageSetting setting = db.ImageSettings.Where(i => i.MaxSize == 1000).FirstOrDefault();
                if (setting != null)
                {
                    db.ImageSettings.Remove(setting);
                    db.Entry(setting).State = EntityState.Deleted;
                    db.SaveChanges();
                }

                // Recursively delete 1000px images
                string filesPath = Server.MapPath("~/Files");
                foreach (string file in Directory.GetFiles(filesPath, "*-1000.jpg",SearchOption.AllDirectories))
                {
                    File.Delete(file);
                }
            }
            // end temp code
            */
        }
    }

    public class MyDataContextDbInitializer : DropCreateDatabaseIfModelChanges<WebContext>
    {
        protected override void Seed(WebContext context)
        {
            // setup image settings
            ImageSetting ProductSmall = new ImageSetting() { BackgroundColor = "White", Key = "ProductImages", MakeSquareImage = true, MaxSize = 100, ShowWatermark = false };
            ImageSetting ProductMedium = new ImageSetting() { BackgroundColor = "White", Key = "ProductImages", MakeSquareImage = true, MaxSize = 300, ShowWatermark = false };
            ImageSetting ProductLarge = new ImageSetting() { BackgroundColor = "White", Key = "ProductImages", MakeSquareImage = true, MaxSize = 600, ShowWatermark = false };
            ImageSetting HtmlContent = new ImageSetting() { Key = "HtmlContent", MaxSize = 600};

            context.ImageSettings.Add(ProductSmall);
            context.ImageSettings.Add(ProductMedium);
            context.ImageSettings.Add(ProductLarge);
            context.ImageSettings.Add(HtmlContent);

            context.SaveChanges();

            

            // setup account registration codes
            for (int i = 0; i < 100; i++)
            {
                AccountRegistrationCode code = new AccountRegistrationCode() { ID = Guid.NewGuid(), Used = false };
                context.RegistrationCodes.Add(code);
            }

            context.SaveChanges();

            // write account codes to text


        }
    } 
}