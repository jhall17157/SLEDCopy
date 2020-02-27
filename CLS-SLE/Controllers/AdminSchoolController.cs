using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text.RegularExpressions;
using CLS_SLE.Models;
using System.Collections;
using CLS_SLE.ViewModels;
using NLog;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminSchoolController : Controller
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();

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

        //// POST: AdminSchool/Create
        //[HttpPost]
        //public ActionResult AddSchool(AddSchoolViewModel addSchoolViewModel, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here
        //        if (ModelState.IsValid)
        //        {
        //            var addSchool = new School(); 
        //            //School addSchool = db.Schools.Create();
        //            {

        //                addSchool.Name = "Name";
        //            ////check if the school name exist before adding it
        //            //if (School.ConvertAll(s => s.name.ToLower()).Contains(name.ToLower()))
        //            //{
        //            //    logger.Info("Duplicate school name ");
                        
        //            //}
        //                addSchool.IsActive = (("IsActive").Equals("True") ? true : false);
        //                addSchool.CreatedDateTime = DateTime.Now;
        //                addSchool.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
        //                addSchool.ModifiedDateTime = DateTime.Now;
        //                addSchool.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());
        //            };
                    
                    

        //            db.Schools.Add(addSchool);
        //            db.SaveChanges();
        //            logger.Info("School id {Id} added", addSchool.SchoolID);//trying to get a message to confirm school is added
        //        }
        //        //return View("Index", db.Schools);
        //        return RedirectToAction(actionName: "Index", controllerName: "AdminSchool");
        //    }
        //    catch
        //    {
        //        logger.Error("Action not completed !!!, redirecting to sign in page.");
        //        return RedirectToAction(actionName: "AddSchool", controllerName: "AdminSchool");
        //    }
        //}

     
    }
}
