using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.ViewModels;
using System.Diagnostics;

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


        //public ActionResult ViewMapping(MappingViewModel mappingViewModel)
        //{
        //    Debug.WriteLine(mappingViewModel.ProgramID + "program id");
        //    Debug.WriteLine(mappingViewModel.Program.Name);


        //    return View(mappingViewModel);
        //}





        ////
        //[HttpPost]
        //public ActionResult GetAssessmentsForProgram(MappingViewModel mappingViewModel)
        //{
        //    Debug.WriteLine(mappingViewModel.ProgramID);
        //    Debug.WriteLine(db.Programs.Where(p => p.ProgramID == mappingViewModel.ProgramID).FirstOrDefault().Name);

        //    if(ModelState.IsValid)
        //    {
        //        //mappingViewModel.Programs = db.Programs.OrderBy(p => p.ProgramID);
        //        mappingViewModel.Program = db.Programs.Where(p => p.ProgramID == mappingViewModel.ProgramID).FirstOrDefault();
        //        mappingViewModel.Assessments = db.Assessments.Where(a => a.ProgramID == mappingViewModel.ProgramID).ToList();
  
        //    }
        //    else
        //    {
                
        //        return RedirectToAction("Index", "AdminMapping");
        //    }
        //    return RedirectToAction("ViewMapping", "AdminMapping");
        //}

        
    }
}