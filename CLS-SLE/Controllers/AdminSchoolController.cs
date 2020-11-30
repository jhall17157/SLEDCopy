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
    public class AdminSchoolController : SLEControllerBase
    {
        SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();

        // GET: AdminSchool
        /// <summary>
        ///       http get request that sends the AdminSchool/Schools view which displays a list of the schools
        /// </summary>
        /// <returns>
        ///       a view of schools that contains a list of schools ordered by the school's name
        /// </returns>
        public ActionResult Schools(string updatedMessage, string addedName) {

            SchoolsViewModel schoolVM = new SchoolsViewModel();
            schoolVM.activeSchools = db.Schools.Where(s => s.IsActive == true).OrderBy(s => s.Name).ToList();
            schoolVM.inActiveSchools = db.Schools.Where(s => s.IsActive != true).OrderBy(s => s.Name).ToList();
            if (updatedMessage != null)
            {
                schoolVM.updatedMessage = updatedMessage;
                if (schoolVM.updatedMessage == "success") { schoolVM.alertMessage = (addedName + " was added!"); }
                else { schoolVM.alertMessage = (addedName + " already exsists!"); }
            }
            return View(schoolVM);
        }
        


        // GET: AdminSchool/AddSchool
        /// <summary>
        ///       http get request that sends the AdminSchool/AddSchool view which displays a form to add a new school to the database
        /// </summary>
        /// <returns>
        ///       a view that contains a submission form for adding a new school
        /// </returns>
        public ActionResult AddSchool(){return View();}

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
            if (db.Schools.Where(s => s.Name == schoolVM.School.Name).FirstOrDefault() == null)
            {
                if (ModelState.IsValid)
                {
                    //Adding created on date
                    schoolVM.School.CreatedDateTime = DateTime.Now;
                    //Adding created by
                    schoolVM.School.CreatedByLoginID = UserData.PersonId;
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
                return RedirectToAction("Schools", "AdminSchool", new { updatedMessage = "success", addedName = schoolVM.School.Name });
            }
            else { return RedirectToAction("Schools", "AdminSchool", new {updatedMessage = "error", addedName = schoolVM.School.Name }); }
        }

        public ActionResult EditSchool(short schoolID)
        {
            ViewBag.school = db.Schools.Where(s => s.SchoolID == schoolID).FirstOrDefault();
            var model = new UpdateSchoolViewModel
            {
                IsActive = ViewBag.school.IsActive
            };
            return View(model);
        }

        public ActionResult ViewSchool(int? schoolId, string updatedMessage)
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
                    try {model.CreatorLogin = (String)db.Users.Where(u => u.PersonID == school.CreatedByLoginID).FirstOrDefault().Login;}
                    catch { model.CreatorLogin = "Unknown"; }   
                }
                if (school.ModifiedByLoginID != null)
                {
                    try { model.ModifierLogin = (String)db.Users.Where(u => u.PersonID == school.CreatedByLoginID).FirstOrDefault().Login; }
                    catch { model.ModifierLogin = "Unknown"; }
                }
                model.school = school;
                if (updatedMessage != null)
                {
                    model.updatedMessage = updatedMessage;
                    if (model.updatedMessage == "success") { model.alertMessage = (model.school.Name + " was updated!"); }
                    else { model.alertMessage = (model.school.Name + " was not updated!"); }
                }
                else { model.updatedMessage = null; }

                return View(model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction("Signin", "User");
            }
        }

        [HttpPost]
        public ActionResult UpdateSchool(UpdateSchoolViewModel schoolVM, short schoolID)
        {

            School editSchool = db.Schools.Where(s => s.SchoolID == schoolID).FirstOrDefault();

            if (ModelState.IsValid)
            {
                editSchool.Name = schoolVM.School.Name;
                editSchool.IsActive = schoolVM.IsActive;
                //Adding modifed on date
                editSchool.ModifiedDateTime = DateTime.Now;
                //Adding modifed by
                editSchool.ModifiedByLoginID = UserData.PersonId;
                //Modifying the school in the database
                db.SaveChanges();
            }
            else
            {
                //redirects user to the submission form if failed to add school
                //TODO figure out how to add form errors
                return RedirectToAction("ViewSchool", "AdminSchool", new { schoolID = schoolID, updatedMessage = "error" });
            }
            //logging that a new school was added
            logger.Info("School id {Id} modified", editSchool.SchoolID);
            //redirects user to the school view if successfully added new school
            return RedirectToAction("ViewSchool", "AdminSchool", new { schoolID = schoolID, updatedMessage = "success" });
        }

        public JsonResult getAllSchools()
        {
            List<School> schools = db.Schools.OrderBy(s => s.Name).ToList();
            if(schools.Count() >0)
            {
                List<SchoolTruncated> allSchoolsOutput = new List<SchoolTruncated>();
                foreach (School s in schools)
                {
                    allSchoolsOutput.Add(new SchoolTruncated() { SchoolID = s.SchoolID, Name = s.Name });
                }
                return new JsonResult { Data = allSchoolsOutput, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return null;
            }
        }
    }
}
