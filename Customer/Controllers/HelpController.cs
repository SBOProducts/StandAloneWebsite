using Customer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Customer.Controllers
{
    public class HelpController : Controller
    {

        public ActionResult Codes()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("AccessDenied", "Account");

            using (WebContext db = new WebContext())
            {
                AccountRegistrationCode[] codes = db.RegistrationCodes.ToArray();
                return View(codes);
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddProducts()
        {
            return View();
        }

        public ActionResult CreateAccount()
        {
            return View();
        }

        public ActionResult Introduction()
        {
            return View();
        }

        public ActionResult InventoryPage()
        {
            return View();
        }

        public ActionResult ModifyContent()
        {
            return View();
        }

    }
}
