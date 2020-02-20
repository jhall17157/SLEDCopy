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
        //public ActionResult Index() => View(db.Schools);
        public ActionResult Index() => View(db.Schools.OrderBy(s => s.SchoolID));

        // GET: AdminSchool
        //public ActionResult Index()
        //{
        //    School s = new School();
        //    List<School> Schools = (from School in db.Schools
        //                            select School).ToList();           
        //    return View();
        //}

        //// GET: AdminSchool/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        // GET: AdminSchool/Create
        public ActionResult AddSchool()
        {
            return View();
        }

        // POST: AdminSchool/Create
        [HttpPost]
        public ActionResult AddSchool(AddSchoolViewModel addSchoolViewModel, FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    var addSchool = new School(); 
                    //School addSchool = db.Schools.Create();
                    {

                        addSchool.Name = "Name";
                    ////check if the school name exist before adding it
                    //if (School.ConvertAll(s => s.name.ToLower()).Contains(name.ToLower()))
                    //{
                    //    logger.Info("Duplicate school name ");
                        
                    //}
                        addSchool.IsActive = (("IsActive").Equals("True") ? true : false);
                        addSchool.CreatedDateTime = DateTime.Now;
                        addSchool.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                        addSchool.ModifiedDateTime = DateTime.Now;
                        addSchool.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                    };
                    
                    

                    db.Schools.Add(addSchool);
                    db.SaveChanges();
                    logger.Info("School id {Id} added", addSchool.SchoolID);//trying to get a message to confirm school is added
                }
                //return View("Index", db.Schools);
                return RedirectToAction(actionName: "Index", controllerName: "AdminSchool");
            }
            catch
            {
                logger.Error("Action not completed !!!, redirecting to sign in page.");
                return RedirectToAction(actionName: "AddSchool", controllerName: "AdminSchool");
            }
        }

        //// GET: AdminSchool/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: AdminSchool/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: AdminSchool/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: AdminSchool/Delete/5
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
    }
}
