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
            mappingViewModel.Rubrics = (from r in db.AssessmentRubrics
                                        select new SelectListItem { Text = r.Name, Value = r.RubricID.ToString() }).Distinct().ToList();
            if(TempData["ProgramID"] != null)
            {
                int programID = (int)TempData["ProgramID"];
                mappingViewModel.Program = db.Programs.FirstOrDefault(p => p.ProgramID == programID);
                mappingViewModel.ProgramID = programID;
            }
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
            mappingViewModel.Rubrics = (from r in db.AssessmentRubrics
                                        select new SelectListItem { Text = r.Name, Value = r.RubricID.ToString() }).Distinct().ToList();

            mappingViewModel.Course = db.Courses.FirstOrDefault(p => p.CourseID == mappingVM.CourseID);
            int programID;
            if(TempData["ProgramID"] != null)
            {
                programID = (int)TempData["ProgramID"]; 
            }
            else
            {
                programID = mappingVM.ProgramID;
            }
            
            mappingViewModel.Program = db.Programs.FirstOrDefault(p => p.ProgramID == programID);
            
            return View(mappingViewModel);
        }


        //create new mapping
        [HttpPost]
        public ActionResult CreateMapping(MappingViewModel mappingVM)
        {
            TempData["ProgramID"] = mappingVM.ProgramID;
            if (ModelState.IsValid)
            {
                ProgramAssessmentMapping map = new ProgramAssessmentMapping();
                map.CourseID = Convert.ToInt16(mappingVM.CourseID);
                map.RubricID = Convert.ToInt16(mappingVM.RubricID);
                map.ProgramID = Convert.ToInt16(mappingVM.ProgramID);
                
                //assigns current date to mapping, then adds it to the database
                map.CreatedDateTime = DateTime.Now;
                if (Session["personID"] != null)
                {
                    map.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                }
                db.ProgramAssessmentMappings.Add(map);
                db.SaveChanges();

            }
            else
            {
                {
                    //return Index(mappingViewModel);
                    return RedirectToAction("Index", "AdminMapping");
                }
            }
            
            //return Index(mappingViewModel);
            return RedirectToAction("Index", "AdminMapping");
        }

        //removes a mapping
        //[HttpPost]
        //public ActionResult DeleteMapping(MappingViewModel mappingVM)
        //{
        //    TempData["ProgramID"] = mappingVM.ProgramID;
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            db.ProgramAssessmentMappings.Remove(mappingVM.Mapping);
        //            db.SaveChanges();
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.WriteLine(e.Message);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "AdminMapping");

        //    }

        //    return RedirectToAction("Index", "AdminMapping");
        //}


        

        [ActionName("DeleteMapping")]
        [HttpPost]
        public ActionResult DeleteMapping(MappingViewModel mappingViewModel, int mapID)
        {
            
            if(ModelState.IsValid)
            {
                var mapping = db.ProgramAssessmentMappings.Where(m => m.ProgramAssessmentMappingID == mapID).FirstOrDefault();
                try
                {
                    db.ProgramAssessmentMappings.Remove(mapping);
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
                TempData["ProgramID"] = mappingViewModel.ProgramID;

            }
            //ProgramAssessmentMapping pamId = db.ProgramAssessmentMappings.Find(mapId);
            //int programId = pamId.ProgramID;
            //db.ProgramAssessmentMappings.Remove(pamId);
            //db.SaveChanges();
            //TempData["ProgramID"] = (int)programId;
            return RedirectToAction("Index", "AdminMapping");
        }



    }
}