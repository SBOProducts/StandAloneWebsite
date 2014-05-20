using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Customer.Models;

namespace Customer.Controllers
{
    public class MyAccountController : Controller
    {

        [Authorize]
        public ActionResult Index()
        {
            using (WebContext db = new WebContext())
            {
                return View(MyAccount.Instance);
            }
        }


        [Authorize]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(MyAccount model)
        {
            if (!ModelState.IsValid)
            {
                model.LogoImage = MyAccount.Instance.LogoImage;
                return View(model);
            }

            MyAccount.Instance.BusinessName = model.BusinessName;
            MyAccount.Instance.EmailAddress = model.EmailAddress;
            MyAccount.Instance.SiteScripts = model.SiteScripts;
            MyAccount.Instance.SiteStyle = model.SiteStyle;
            MyAccount.Instance.Save();

            ViewBag.Message = "Your changes have been saved";
            return View(MyAccount.Instance);
        }

        [Authorize]
        public ActionResult ContactFormPostings()
        {
            using (WebContext db = new WebContext())
            {
                ContactForm[] entries = db.ContactForms.OrderByDescending(p=>p.Sent).ToArray();
                return PartialView(entries);
            }
        }

    }
}
