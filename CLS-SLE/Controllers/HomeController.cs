using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.Models;
using CLS_SLE.Security;
using System.Web.Security;
using System.Net.Mail;

namespace CLS_SLE.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //GET: Home/SignOut
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction(actionName: "SignedOut", controllerName: "Home");
        }

        //GET: Home/SignedOut
        public ActionResult SignedOut()
        {
            return View();
        }

        //GET: Home/CheckEmail
        public ActionResult CheckEmail()
        {
            return View();
        }
    }
}