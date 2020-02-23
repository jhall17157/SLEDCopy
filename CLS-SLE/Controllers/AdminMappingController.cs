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
            var availablePrograms = db.Programs;
            mappingViewModel.AvailablePrograms = new SelectList(availablePrograms, "ProgramID", "Name");
            return View(mappingViewModel);
        }

        //returns index page with viewmodel populated by selected program
        [HttpPost]
        public ActionResult Index(MappingViewModel mappingVM)
        {
            int programID = mappingVM.SelectedProgram.GetValueOrDefault();
            MappingViewModel mappingViewModel = new MappingViewModel();
            mappingViewModel.Programs = db.Programs;
            mappingViewModel.Courses = db.Courses;
            mappingViewModel.Program = mappingViewModel.Programs.Where(p => p.ProgramID == programID).FirstOrDefault();
            mappingViewModel.SelectedProgram = programID;
            mappingViewModel.AvailablePrograms = new SelectList(mappingViewModel.Programs, "ProgramID", "Name");
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
        



    }
}