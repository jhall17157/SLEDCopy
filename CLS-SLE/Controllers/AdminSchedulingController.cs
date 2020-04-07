using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using NLog.LayoutRenderers.Wrappers;

namespace CLS_SLE.Controllers
{
    public class AdminSchedulingController : Controller
    {
        
        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        // GET: Admin Scheduling home page with list of semesters to choose from.
        public ActionResult Index()
        {
            SchedulingViewModel schedulingViewModel = new SchedulingViewModel();
            schedulingViewModel.Semesters = (from s in db.Semesters
                    select new SelectListItem {Text = s.SemesterCode + " " + s.Name, Value = s.SemesterID.ToString()})
                        .Distinct().ToList();

            return View(schedulingViewModel);
        }

        [HttpPost]
        public ActionResult Index(SchedulingViewModel viewModel)
        {
            SchedulingViewModel schedulingViewModel = new SchedulingViewModel();
            schedulingViewModel.People = db.People;
            schedulingViewModel.Semesters = (from s in db.Semesters
                    select new SelectListItem { Text = s.SemesterCode + " " + s.Name, Value = s.SemesterID.ToString() })
                .Distinct().ToList();
            schedulingViewModel.Semester =
                db.Semesters.FirstOrDefault(s => s.SemesterID == viewModel.SemesterID);



            return View(schedulingViewModel);
        }


        public ActionResult TimeframeSemester()
        {
            return View("TimeframeSemester");
        }
    }
}