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

        // GET: AdminSchool
        /// <summary>
        ///       http get request that sends the AdminSchool/Schools view which displays a list of the schools
        /// </summary>
        /// <returns>
        ///       a view of schools that contains a list of schools ordered by the school's schoolID
        /// </returns>
        public ActionResult Schools() => View(db.Schools.OrderBy(s => s.SchoolID));

        // GET: AdminSchool/AddSchool
        /// <summary>
        ///       http get request that sends the AdminSchool/AddSchool view which displays a form to add a new school to the database
        /// </summary>
        /// <returns>
        ///       a view that contains a submission form for adding a new school
        /// </returns>
        public ActionResult AddSchool() { return View(); }

        // POST: AdminSchool/CreateSchool
        /// <summary>
        ///       http post request that returns the data from the AddSchool submission form in the form of an AddSchoolViewModel
        ///       than confirms that the model is valid, if valid the created date and create by fields are added and the hashed and passed into the database
        /// </summary>
        /// <param name="schoolVM"></param>
        /// <returns>
        ///       returns the user to the list of schools if successful otherwise returns users back to the AddSchool submission form
        /// </returns>
        [HttpPost]
        public ActionResult CreateSchool(AddSchoolViewModel schoolVM)
        {
            if (ModelState.IsValid)
            {
                //Adding created on date
                schoolVM.School.CreatedDateTime = DateTime.Now;
                //Adding created by
                //TODO add created by
                schoolVM.School.CreatedDateTime = DateTime.Now;
                //Adding the new school to the database
                db.Schools.Add(schoolVM.School);
                db.SaveChanges();
            }
            else
            {
                //redirects user to the submission form if failed to add school
                //TODO figure out how to add form errors
                return RedirectToAction("AddSchool", "AdminSchool");
            }
            //logging that a new school was added
            logger.Info("School id {Id} added", schoolVM.School.SchoolID);
            //redirects user to the list of schools if successfully added new school
            return RedirectToAction("Schools", "AdminSchool");
        }
    }
}

