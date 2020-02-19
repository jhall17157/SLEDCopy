using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminSchoolController : Controller
    {
        SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();

        //view school details 
        //list departments
        //edit school info

        public ActionResult Index() => View(db.Schools.OrderBy(s => s.Name));

        public ActionResult Create() => View();

        public ActionResult Edit(short id)
        {
            School school = db.Schools.Where(s => s.SchoolID == id).FirstOrDefault();
            ViewBag.Id = school.SchoolID;
            ViewBag.SchoolName = school.Name;

            return View();
        }

        public ActionResult ViewSchool(int? schoolId)
        {
            var school = new School();

            try
            {
                if (schoolId.HasValue)
                {
                    school = db.Schools.FirstOrDefault(s => s.SchoolID == schoolId.Value);
                }

                dynamic model = new ExpandoObject();
                model.CreatorLogin = null;
                model.ModifierLogin = null;

                if (school.CreatedByLoginID != null)
                {
                    model.CreatorLogin = (String)db.Users.Where(u => u.PersonID == school.CreatedByLoginID).FirstOrDefault().Login;
                }
                if (school.ModifiedByLoginID != null)
                {
                    model.ModifierLogin = (String)db.Users.Where(u => u.PersonID == school.ModifiedByLoginID).FirstOrDefault().Login;
                }
                model.school = school;

                return View(model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction("Signin", "User");
            }
        }

        [HttpPost]
        public ActionResult UpdateSchool(SchoolUpdateViewModel schoolUpdateViewModel, short id)
        {
            School editSchool = db.Schools.Where(s => s.SchoolID == id).FirstOrDefault();

            editSchool.Name = schoolUpdateViewModel.School.Name;

            db.SaveChanges();

            return RedirectToAction("ViewSchools", "Admin");
        }

        public ActionResult Activate(SchoolDetailViewModel schoolDetailViewModel, short id)
        {
            schoolDetailViewModel.School = db.Schools.Where(s => s.SchoolID == id).SingleOrDefault();

            if (schoolDetailViewModel.School.IsActive)
            {
                schoolDetailViewModel.School.IsActive = false;
                db.SaveChanges();
            }
            else
            {
                schoolDetailViewModel.School.IsActive = true;
                db.SaveChanges();
            }

            return RedirectToAction("ViewSchools", "Admin");
        }
    }
}