using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    public class MockupController : Controller
    {
        public ActionResult FacultyLandingPage()
        {
            return View();
        }

        public ActionResult TSAAssessment()
        {
            return View();
        }

        public ActionResult TSAAssessment2()
        {
            return View();
        }

        public ActionResult CLSAssessment()
        {
            return View();
        }

        public ActionResult AdministratorLandingPage()
        {
            return View();
        }
    }
}