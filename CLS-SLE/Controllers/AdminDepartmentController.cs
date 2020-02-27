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
    public class AdminDepartmentController : Controller
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();

        // GET: AdminDeaprtment/Departments
        /// <summary>
        ///       http get request that sends the AdminDepartment/Departments view which displays a list of the departments
        /// </summary>
        /// <returns>
        ///       a view of departments that contains a list of departments ordered by the department's departmentID
        /// </returns>
        public ActionResult Departments() => View(db.Departments.OrderBy(s => s.DepartmentID));

        // GET: AdminDepartment/AddDepartment
        /// <summary>
        ///       http get request that sends the AdminDepartment/AddDepartment view which displays a form to add a new department to the database
        /// </summary>
        /// <returns>
        ///       a view that contains a submission form for adding a new department
        /// </returns>
        public ActionResult AddDepartment() { return View(); }

        public ActionResult ViewDepartment(short id) { return View(db.Departments.Where(d => d.DepartmentID == id).FirstOrDefault()); }
        public ActionResult EditDepartment(short id) {

            dynamic model = new ExpandoObject();

            ViewBag.Schools = db.Schools;
            ViewBag.Department = db.Departments.Where(d => d.DepartmentID == id).FirstOrDefault();

            return View(); 
        }


        // POST: AdminDepartment/CreateDepartment
        /// <summary>
        ///       http post request that returns the data from the AddDepartment submission form in the form of an AddDepartmentViewModel
        ///       than confirms that the model is valid, if valid the created date and create by fields are added and the hashed and passed into the database
        /// </summary>
        /// <param name="departmentVM"></param>
        /// <returns>
        ///       returns the user to the list of departments if successful otherwise returns users back to the AddDepartment submission form
        /// </returns>
        [HttpPost]
        public ActionResult CreateDepartment(AddDepartmentViewModel departmentVM)
        {
            if (ModelState.IsValid)
            {
                //Adding created on date
                departmentVM.Department.CreatedDateTime = DateTime.Now;
                //Adding created by
                //TODO add created by
                departmentVM.Department.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                //Adding the new department to the database
                db.Departments.Add(departmentVM.Department);
                db.SaveChanges();
            }
            else
            {
                //redirects user to the submission form if failed to add department
                //TODO figure out how to add form errors
                return RedirectToAction("AddDepartment", "AdminDepartment");
            }
            //logging that a new department was added
            logger.Info("School id {Id} added", departmentVM.Department.DepartmentID);
            //redirects user to the list of departments if successfully added new 
            return RedirectToAction("Departments", "AdminDepartment");
        }

        [HttpPost]
        public ActionResult UpdateDepartment(EditDepartmentViewModel departmentVM, short id)
        {
            Department editDepartment = db.Departments.Where(d => d.DepartmentID == id).FirstOrDefault();

            editDepartment.Name = departmentVM.Department.Name;
            editDepartment.Number = departmentVM.Department.Number;
            editDepartment.IsActive = departmentVM.Department.IsActive;

            if(editDepartment.School != departmentVM.Department.School)
            {
                School editSchool = db.Schools.Where(s => s.SchoolID == departmentVM.Department.School.SchoolID).FirstOrDefault();

                editSchool.Departments.Add(departmentVM.Department);
                editDepartment.School = departmentVM.Department.School;
            }

            editDepartment.ModifiedDateTime = DateTime.Now;
            editDepartment.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());

            db.SaveChanges();

            return RedirectToAction("ViewUsers", "Admin");
        }
    }
}