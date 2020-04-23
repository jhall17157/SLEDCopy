using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        //Called when user selects a semester
        [HttpPost]
        public ActionResult Index(SchedulingViewModel viewModel)
        {
            SchedulingViewModel schedulingViewModel = new SchedulingViewModel();
            //schedulingViewModel.People = db.People;
            schedulingViewModel.Semesters = (from s in db.Semesters
                    select new SelectListItem { Text = s.SemesterCode + " " + s.Name, Value = s.SemesterID.ToString() })
                .Distinct().ToList();
            schedulingViewModel.Semester =
                db.Semesters.FirstOrDefault(s => s.SemesterID == viewModel.SemesterID);

            //var result = from sr in db.SectionRubrics
            //              join sec in db.Sections on sr.SectionID equals sec.SectionID
            //              join sem in db.Semesters on sec.SemesterID equals sem.SemesterID
            //              join ar in db.AssessmentRubrics on sr.RubricID equals ar.RubricID
            //              join c in db.Courses on sec.CourseID equals c.CourseID
            //              where sem.SemesterID == schedulingViewModel.Semester.SemesterID
            //              select new
            //              {                              
            //                  semester = sem,
            //                  course = c.CourseName,
            //                  crn = sec.CRN,
            //                  rubric = ar.Name,
            //                  start = sr.StartDate,
            //                  end = sr.EndDate
            //              };
            

            return View(schedulingViewModel);
        }
        public ActionResult NewSemester()
        {

            return View();

        }
        [HttpPost]
        public ActionResult CreateSemesterSchedule(SchedulingViewModel viewModel)
        {
            //check model state before continuing
            if(ModelState.IsValid)
            {
                //pull semester info and default start and end dates from view model
                Semester semester = viewModel.Semester;
                DateTime StartDate = viewModel.StartDate;
                DateTime EndDate = viewModel.EndDate;
                                                          
                //based on selected semester, grab all rubrics from mappings and sections from sections tables
                var result = from m in db.ProgramAssessmentMappings
                             join s in db.Sections on m.CourseID equals s.CourseID
                             where s.SemesterID == semester.SemesterID
                             select new
                             {
                                 s.SectionID,
                                 m.RubricID
                             };
                //ensure results are distinct to avoid duplicate entries
                result = result.Distinct();
                
                //build list of schedule entries from result set
                List<SectionRubric> Schedules = new List<SectionRubric>();
                                
                foreach (var item in result)
                {
                    SectionRubric sectionRubric = new SectionRubric();
                    sectionRubric.SectionID = item.SectionID;
                    sectionRubric.RubricID = item.RubricID;
                    sectionRubric.StartDate = StartDate;
                    sectionRubric.EndDate = EndDate;
                    sectionRubric.CreatedDateTime = DateTime.Now;
                    if(Session["personID"] != null)
                    {
                        sectionRubric.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                    }

                    Schedules.Add(sectionRubric);
                }
                //Check if new schedule list is empty
                if(Schedules.Any())
                {
                    foreach(SectionRubric sectionRubric in Schedules)
                    {
                        try
                        {
                            db.SectionRubrics.Add(sectionRubric);
                        }
                        catch(Exception e)
                        {
                            Debug.WriteLine(e.ToString());
                            Debug.WriteLine("Failed Scheduling information: \n" +
                                "   SectionID: " + sectionRubric.SectionID + "\n" +
                                "   RubricID: " + sectionRubric.RubricID + "\n" +
                                "   Start Date: " + sectionRubric.StartDate.ToString() + "\n" +
                                "   End Date: " + sectionRubric.EndDate.ToString());
                            continue;
                        }
                        
                    }
                    db.SaveChanges();
                }

            }
            else
            {
                return RedirectToAction("Index", "AdminScheduling");
            }
            return RedirectToAction("Index", "AdminScheduling");
        }


        public ActionResult TimeframeSemester()
        {
            return View("TimeframeSemester");
        }
    }
}

//select sem.Name, c.CourseName, sec.CRN, ar.Name, sr.StartDate, sr.EndDate
//from[SLE_Tracking].[dbo].SectionRubric sr
//join[SLE_Tracking].[dbo].Section sec on sr.SectionID = sec.SectionID
//join [SLE_Tracking].[dbo].Semester sem on sec.SemesterID = sem.SemesterID
//join[SLE_Tracking].[dbo].AssessmentRubric ar on sr.RubricID = ar.RubricID
//join[SLE_Tracking].[dbo].Course c on sec.CourseID = c.CourseID
//where sem.SemesterID = 4
//order by sem.Name, c.CourseName, sec.CRN, ar.Name
