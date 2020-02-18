using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.ViewModels;
using System.Diagnostics;
using System.Data.Entity;

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
            var availablePrograms = db.Programs;
            mappingViewModel.AvailablePrograms = new SelectList(availablePrograms, "ProgramID", "Name");
            
            return View(mappingViewModel);
        }

        //[HttpPost]
        //public ActionResult Index(MappingViewModel model) => View(db.ProgramAssessmentMappings.Where(pam => pam.ProgramID == model.SelectedProgram.GetValueOrDefault()));

        [HttpPost]
        public ActionResult Index(MappingViewModel model)
        {

            
            //IQueryable data = from program in db.Programs
            //                  join assessment in db.Assessments on program.ProgramID equals assessment.ProgramID
            //                  join rubricAssessment in db.RubricAssessments on assessment.AssessmentID equals rubricAssessment.AssessmentID
            //                  join rubric in db.AssessmentRubrics on rubricAssessment.RubricID equals rubric.RubricID
            //                  join programAssessment in db.ProgramAssessmentMappings on rubric.RubricID equals programAssessment.RubricID
            //                  join course in db.Courses on programAssessment.CourseID equals course.CourseID
                              
                              
                              


            int programID = model.SelectedProgram.GetValueOrDefault();


            MappingViewModel mappingViewModel = new MappingViewModel();
            mappingViewModel.Programs = db.Programs;
            mappingViewModel.Program = mappingViewModel.Programs.Where(p => p.ProgramID == programID).FirstOrDefault();

            mappingViewModel.SelectedProgram = programID;
            
            
            mappingViewModel.AvailablePrograms = new SelectList(mappingViewModel.Programs, "ProgramID", "Name");
            return View(mappingViewModel);
        }


        //public ActionResult Index(MappingViewModel mappingViewModel)
        //{
        //    return View(mappingViewModel);
        //}

        //Gets mappings for specified program's assessments
        //[HttpGet]
        //public IEnumerable<Assessment> GetAssessments(int programID) => db.Assessments.Where(a => a.ProgramID == programID);



        //[HttpGet]
        //public IEnumerable<ProgramAssessmentMapping> GetMappings(int programID) => db.ProgramAssessmentMappings.Where(p => p.ProgramID == programID);
        ////{

        //    var result = db.Assessments.Include("Courses").Where(a => a.ProgramID == id);
        //    return result;

        //}




        //public ActionResult ViewMapping(MappingViewModel mappingViewModel)
        //{
        //    Debug.WriteLine(mappingViewModel.ProgramID + "program id");
        //    Debug.WriteLine(mappingViewModel.Program.Name);


        //    return View(mappingViewModel);
        //}






        ////[HttpPost]
        //public ActionResult ViewMappings(MappingViewModel mappingViewModel)
        //{


        //    if (ModelState.IsValid)
        //    {
        //        //mappingViewModel.Programs = db.Programs.OrderBy(p => p.ProgramID);
        //        mappingViewModel.Program = db.Programs.Where(p => p.ProgramID == mappingViewModel.SelectedProgram).FirstOrDefault();
        //        mappingViewModel.Assessments = db.Assessments.Where(a => a.ProgramID == mappingViewModel.SelectedProgram).ToList();

        //    }
        //    else
        //    {

        //        return RedirectToAction("Index", "AdminMapping");
        //    }
        //    return View("Index", mappingViewModel);
        //}

        //todo: figure out how to pass model to back to view
    }
}