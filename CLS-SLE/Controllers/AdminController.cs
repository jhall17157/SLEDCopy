using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdminDashboard()
        {
            return View();
        }

        public ActionResult AssessmentMappings()
        {
            return View();
        }

        public ActionResult Assessments()
        {
            return View();
        }

        public ActionResult AssessmentScheduling()
        {
            return View();
        }

        public ActionResult ProgramsCourses()
        {
            return View();
        }

        public ActionResult SchoolsDepartments()
        {
            return View();
        }

        public ActionResult UsersSecurity()
        {
            return View();
        }

    }
}