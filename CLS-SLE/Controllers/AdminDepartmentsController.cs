using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminDepartmentsController : Controller
    {
        SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();
        // GET: AdminDepartments
        public ActionResult Departments() => View(db.Departments.OrderByDescending(d => d.DepartmentID).ThenBy(d => d.SchoolID == d.DepartmentID));
        //View department
        public ActionResult ViewDepartment(short id, ProgramDepartment pg)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(db.Departments.Where(d => d.DepartmentID == id).FirstOrDefault());
        }
        // GET: AdminDepartments/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminDepartments/Create
        public ActionResult CreateDepartment(AddDepartmentViewModel departmentVM)
        {
            Department department = db.Departments.Where(d => d.DepartmentID == d.SchoolID).FirstOrDefault();


            ViewBag.Departments = db.Departments.Select(d => d.Name).ToList();
            ViewBag.School = db.Departments.Where(d => d.DepartmentID == d.SchoolID).FirstOrDefault().Name;
            ViewBag.DepartmentID = department;

            return View();
        }

        // POST: AdminDepartments/Create
        [HttpPost]
        public ActionResult CreateDepartment(FormCollection formCollection, AddDepartmentViewModel departmentVM)
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
                    if (department.School != departmentVM.Department.School)
                    {
                        School deptSchool = db.Schools.Where(s => s.SchoolID == departmentVM.Department.School.SchoolID).FirstOrDefault();

                        deptSchool.Departments.Add(departmentVM.Department);
                        department.School = departmentVM.Department.School;
                    }
                    department.IsActive = departmentVM.Department.IsActive;
                    department.CreatedDateTime = DateTime.Now;
                    department.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                    db.Departments.Add(department);
                    db.SaveChanges();

                    logger.Info("Department id {Id} added", departmentVM.Department.DepartmentID);
                    //object schoolID = null;
                    //ViewData["SchoolID"] = schoolID;
                    return RedirectToAction("Departments", "AdminDepartments");
                }

                else
                {
                    //TODO figure out how to add form errors
                    return RedirectToAction("CreateDepartment", "AdminDepartments");
                }
            }
            catch
            {
                logger.Error("Check the entered credentials and retry.");
                return RedirectToAction("CreateDepartment", "AdminDepartments");
            }
        }

        // GET: AdminDepartments/Edit/5
        public ActionResult EditDepartment(short id)
        {
            ViewBag.Schools = db.Schools;
            ViewBag.Department = db.Departments.Where(d => d.DepartmentID == id).FirstOrDefault();
            return View();
        }

        // POST: AdminDepartments/Edit/5
        [HttpPost]
        public ActionResult EditDepartment(int? id, FormCollection collection, EditDepartmentViewModel departmentVM, short DepartmentID)
        {
            try
            {
                /*if (!DepartmentID.Equals(null)) */   // I think this might return null since I didn't firdt create a null object
                //if (DepartmentID != null)
                if (DepartmentID.Equals(true))
                {
                    Department editDepartment = db.Departments.Where(d => d.DepartmentID == id).FirstOrDefault();

                    editDepartment.Name = departmentVM.Department.Name;
                    editDepartment.Number = departmentVM.Department.Number;
                    editDepartment.IsActive = departmentVM.Department.IsActive;

                    if (editDepartment.School != departmentVM.Department.School)
                    {
                        School editSchool = db.Schools.Where(s => s.SchoolID == departmentVM.Department.School.SchoolID).FirstOrDefault();

                        editSchool.Departments.Add(departmentVM.Department);
                        editDepartment.School = departmentVM.Department.School;
                    }

                    editDepartment.ModifiedDateTime = DateTime.Now;
                    editDepartment.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                    db.SaveChanges();

                    logger.Info("Department id {Id} Succesfully edited", departmentVM.Department.DepartmentID);
                    return RedirectToAction("Departments", "AdminDepartments");
                }
                else
                {
                    throw new Exception("Invalid Credentials!!!");
                    //return RedirectToAction("EditDepartment", "AdminDepartments");
                }

                //return RedirectToAction("EditDepartment", "AdminDepartments");
            }
            catch
            {
                logger.Error("Check the entered credentials and retry.");
                return RedirectToAction("EditDepartment", "AdminDepartments");
            }
        }

        // GET: AdminDepartments/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: AdminDepartments/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //
        public ActionResult Activate(AddDepartmentViewModel departmentVM, short id)
        {
            departmentVM.Department = db.Departments.Where(d => d.DepartmentID == id).SingleOrDefault();

            if (departmentVM.Department.IsActive)
            {
                departmentVM.Department.IsActive = false;
                db.SaveChanges();
            }
            else
            {
                departmentVM.Department.IsActive = true;
                db.SaveChanges();
            }

            return RedirectToAction("ViewDepartments", "Admin");
        }
    }
}
