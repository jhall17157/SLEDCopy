using System;
using System.Diagnostics;
using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{

    //New controller for mapping page
    public class AdminMappingController : Controller
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();


        // GET: AdminMapping home page with list of Programs sorted by id
        public ActionResult Index()
        {
            MappingViewModel mappingViewModel = new MappingViewModel();
            mappingViewModel.Programs = (from p in db.Programs
                                         select new SelectListItem { Text = p.Number + " " + p.Name, Value = p.ProgramID.ToString() })
                                            .Distinct().ToList();
            mappingViewModel.Courses = (from c in db.Courses
                                        where !c.CourseName.Contains("Folio180")
                                        select new SelectListItem { Text = c.Number + " " + c.CourseName, Value = c.CourseID.ToString() })
                                            .Distinct().ToList();
            return View(mappingViewModel);
        }

        //returns index page with viewmodel populated by selected program
        [HttpPost]
        public ActionResult Index(MappingViewModel mappingVM)
        {
            
            MappingViewModel mappingViewModel = new MappingViewModel();
            mappingViewModel.Programs = (from p in db.Programs
                                         select new SelectListItem { Text = p.Number + " " + p.Name, Value = p.ProgramID.ToString() })
                                            .Distinct().ToList();
            mappingViewModel.Courses = (from c in db.Courses
                                        where !c.CourseName.Contains("Folio180")
                                        select new SelectListItem { Text = c.Number + " " + c.CourseName, Value = c.CourseID.ToString() })
                                            .Distinct().ToList();

            mappingViewModel.Course = db.Courses.FirstOrDefault(p => p.CourseID == mappingVM.CourseID);
            mappingViewModel.Program = db.Programs.FirstOrDefault(p => p.ProgramID == mappingVM.ProgramID);
            
            return View(mappingViewModel);
        }


        //create new mapping
        [HttpPost]
        public ActionResult CreateMapping(MappingViewModel mappingVM)
        {
            if (ModelState.IsValid)
            {
                
                //assigns current date to mapping, then adds it to the database
                mappingVM.Mapping.CreatedDateTime = DateTime.Now;
                db.ProgramAssessmentMappings.Add(mappingVM.Mapping);
                db.SaveChanges();

            }
            else
            {
                {
                    return RedirectToAction("Index", "AdminMapping");
                }
            }

            return RedirectToAction("Index", "AdminMapping");
        }

        //removes a mapping
        [HttpPost]
        public ActionResult DeleteMapping(MappingViewModel mappingVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.ProgramAssessmentMappings.Remove(mappingVM.Mapping);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            else
            {
                return RedirectToAction("Index", "AdminMapping");

            }

            return RedirectToAction("Index", "AdminMapping");
        }


        //Delete method does not account for confirmation modal. 
            //Need to bind @Model.MappingID = @pam.ProgramAssessmentID
        [ActionName("DeleteMapping")]
        public ActionResult DeleteMapping(short id)
        {
            ProgramAssessmentMapping pamId = db.ProgramAssessmentMappings.Find(id);
            db.ProgramAssessmentMappings.Remove(pamId);
            db.SaveChanges();

            return RedirectToAction("Index", "AdminMapping");
        }



    }
}