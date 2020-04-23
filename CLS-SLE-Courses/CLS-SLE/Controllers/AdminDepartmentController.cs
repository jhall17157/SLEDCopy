﻿using System;
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
using System.Net;

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
        public ActionResult Departments() => View(db.Departments.OrderBy(d => d.Name));

        // GET: AdminDepartment/AddDepartment
        /// <summary>
        ///       http get request that sends the AdminDepartment/AddDepartment view which displays a form to add a new department to the database
        /// </summary>
        /// <returns>
        ///       a view that contains a submission form for adding a new department
        /// </returns>
        public ActionResult AddDepartment() {

            AddDepartmentViewModel departmentVM = new AddDepartmentViewModel();
            List<String> schoolNames = new List<String>();

            foreach(var s in db.Schools) { schoolNames.Add(s.Name); }
            departmentVM.SchoolNames = schoolNames;
            
            return View(departmentVM); }

        public ActionResult ViewDepartment(short? id) {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            return View(db.Departments.Where(d => d.DepartmentID == id).FirstOrDefault()); }

        public ActionResult EditDepartment(short? id) {
            //if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            EditDepartmentViewModel departmentVM = new EditDepartmentViewModel();
            List<String> schoolNames = new List<String>();

            foreach (var s in db.Schools) { schoolNames.Add(s.Name); }
            departmentVM.SchoolNames = schoolNames;
            departmentVM.Department = db.Departments.Where(d => d.DepartmentID == id).FirstOrDefault();
            return View(departmentVM);

            //ViewBag.department = db.Departments.Where(d => d.DepartmentID == DepartmentID).FirstOrDefault();
            //var departmentVM = new EditDepartmentViewModel
            //{
            //    //foreach (var s in db.Schools) { schoolNames.Add(s.Name); }
            //    SchoolNames = ViewBag.department.SchoolNames,
            //    //DepartmentName = ViewBag.department.DepartmentName,
            //};
            //return View(departmentVM);

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
            try
            {
                if (ModelState.IsValid)
                {
                    db.Departments.Load();

                    Department createDepartment = db.Departments.Create();
                    Department department = new Department();

                    department.DepartmentID = department.DepartmentID;
                    department.Number = departmentVM.Department.Number;
                    department.Name = departmentVM.Department.Name;
                    department.School = db.Schools.Where(s => s.Name == departmentVM.SchoolSelection).OrderByDescending(s => s.Name).FirstOrDefault();
                    //department.School = new SelectList(db.Schools.Select(s => new { Id = s.SchoolID, Name = s.Name }), "ID", "Name", "[Select Name]");
                    department.IsActive = departmentVM.Department.IsActive;
                    department.CreatedDateTime = DateTime.Now;
                    department.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                    db.Departments.Add(department);
                    db.SaveChanges();

                    logger.Info("Department id {Id} added", departmentVM.Department.DepartmentID);
                    //object schoolID = null;
                    //ViewData["SchoolID"] = schoolID;
                    return RedirectToAction("Departments", "AdminDepartment");
                }

                else
                {
                    //TODO figure out how to add form errors
                    return RedirectToAction("CreateDepartment", "AdminDepartment");
                }
            }
            catch
            {
                logger.Error("Check the entered credentials and retry.");
                return RedirectToAction("CreateDepartment", "AdminDepartment");
            }
        }

        [HttpPost]
        public ActionResult UpdateDepartment(EditDepartmentViewModel departmentVM, short DepartmentID)
        {
            try
            {
                /*if (!DepartmentID.Equals(null)) */   // I think this might return null since I didn't firdt create a null object
                                                       //if (DepartmentID != null)
                Department editDepartment = db.Departments.Where(d => d.DepartmentID == DepartmentID).FirstOrDefault();
                if (ModelState.IsValid)
                {              
                    if (editDepartment.School != departmentVM.Department.School)
                        {
                            School editSchool = db.Schools.Where(s => s.SchoolID == departmentVM.Department.School.SchoolID).FirstOrDefault();
                            editSchool.Departments.Add(departmentVM.Department); 
                            editDepartment.School = departmentVM.Department.School;
                        }
                    editDepartment.Name = departmentVM.Department.Name;
                    editDepartment.Number = departmentVM.Department.Number;
                    editDepartment.IsActive = departmentVM.Department.IsActive;

                    

                    editDepartment.ModifiedDateTime = DateTime.Now;
                    editDepartment.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                    db.SaveChanges();

                    //logger.Info("Department id {Id} Succesfully edited", departmentVM.Department.DepartmentID);
                    //return RedirectToAction("Departments", "AdminDepartment");
                }

                else
                {
                    logger.Error("Check the entered credentials and retry.");
                    return RedirectToAction("Departments", "AdminDepartment");
                }
                //return RedirectToAction("Departments", "AdminDepartment");
                //return RedirectToAction("ViewDepartment", "AdminDepartment", new { id = departmentVM.Department.DepartmentID });
            }
            //return RedirectToAction("Departments", "AdminDepartment");
            catch (Exception ex)
            {
                logger.Info("Check the entered credentials and retry.");
                logger.Error("Exception: " + ex.Message);
                return RedirectToAction("Departments", "AdminDepartment");
            }
            return RedirectToAction("Departments", "AdminDepartment");
        }
        //Alert for submit
        public ActionResult Submit(string submit)
        {
            ViewBag.Message = "Successful!!!";
            return View();
        }
    }
}