using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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

        private readonly int pageSize = 10;

        // GET: Admin Scheduling home page with list of semesters to choose from.
        public ActionResult Index()
        {
            SchedulingViewModel schedulingViewModel = new SchedulingViewModel();
            schedulingViewModel.Semesters = GetSemesters(true);
            if (TempData["SemesterID"] != null)
            {
                schedulingViewModel.SemesterID = (int)TempData["SemesterID"];
                return Index(schedulingViewModel, 1);
            }
            return View(schedulingViewModel);
        }

        //Called when user selects a semester 
        [HttpPost]
        public ActionResult Index(SchedulingViewModel viewModel, int page = 1)
        {
            

            SchedulingViewModel schedulingViewModel = new SchedulingViewModel();
            
            //Defining Rubrics
            schedulingViewModel.AssesmentRubrics = (from r in db.AssessmentRubrics
                                                    select new SelectListItem { Text = r.Name, Value = r.RubricID.ToString() }).Distinct().ToList();

            if (TempData["SemesterID"] != null)
            {
                schedulingViewModel.SemesterID = (int)TempData["SemesterID"];
            }
            else
            {
                schedulingViewModel.SemesterID = viewModel.SemesterID;
            }            
            schedulingViewModel.Semester = db.Semesters.FirstOrDefault(s => s.SemesterID == schedulingViewModel.SemesterID);

            schedulingViewModel.Semesters = GetSemesters(true);
            
            //get course ids for all sections in selected semester that have assessments scheduled and returns as a list 
            List<short> courseIDs = schedulingViewModel.Semester.Sections.Where(c => c.SectionRubrics.Count > 0).Select(i => i.CourseID).Distinct().ToList();
            //handles pagination
            schedulingViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = courseIDs.Count()
                
            };
            //get all courses whose courseIDs exist in provided list of courseids
            var courseResult = db.Courses
                .Where(c => courseIDs.Contains(c.CourseID));

            if(viewModel.CourseID != null && viewModel.CourseID != -1)
            {
                courseResult = courseResult.Where(c => c.CourseID == viewModel.CourseID);
            }
            schedulingViewModel.Courses = courseResult.OrderBy(c => c.Number).ToList();
            schedulingViewModel.CourseSelectList = new List<SelectListItem>();
            schedulingViewModel.CourseSelectList.Add(new SelectListItem { Text = "All Courses For Semester", Value = "-1" });

            foreach(Course course in schedulingViewModel.Courses)
            {
                schedulingViewModel.CourseSelectList.Add(new SelectListItem { Text = course.Number + " " + course.CourseName, Value = course.CourseID.ToString() });
            }

            schedulingViewModel.Courses = schedulingViewModel.Courses
                .Skip((schedulingViewModel.PagingInfo.CurrentPage - 1) * schedulingViewModel.PagingInfo.ItemsPerPage)
                .Take(schedulingViewModel.PagingInfo.ItemsPerPage).ToList();

            ModelState.Clear();
            return View(schedulingViewModel);
        }

        public ActionResult ScheduleSemester()
        {
            ScheduleSemesterViewModel scheduleSemesterViewModel = new ScheduleSemesterViewModel();

            scheduleSemesterViewModel.Semesters = GetSemesters(false);

            return View(scheduleSemesterViewModel);
        }

        [HttpPost]
        public ActionResult CreateSemesterSchedule(ScheduleSemesterViewModel viewModel)
        {
            //check model state before continuing
            if(ModelState.IsValid)
            {

                
                //pull semester info and default start and end dates from view model
                Semester semester = viewModel.Semester;
                

                //based on selected semester, grab all rubrics from mappings and sections from sections tables
                var result = from m in db.ProgramAssessmentMappings
                             join s in db.Sections on m.CourseID equals s.CourseID
                             where s.SemesterID == semester.SemesterID
                             select new
                             {
                                 s.SectionID,
                                 s.BeginDate,
                                 s.EndDate,
                                 m.RubricID,                                 
                             };
                
                //ensure results are distinct to avoid duplicate entries
                result = result.Distinct();
                
                //build list of schedule entries from result set
                List<SectionRubric> Schedules = new List<SectionRubric>();
                
                //Iterate through result set
                foreach (var item in result)
                {
                    SectionRubric sectionRubric = new SectionRubric();
                    sectionRubric.SectionID = item.SectionID;
                    sectionRubric.RubricID = item.RubricID;
                    //check if dates are set by specific date or based on section end date
                    if(viewModel.isDates)
                    {
                        sectionRubric.StartDate = viewModel.StartDate;
                        sectionRubric.EndDate = viewModel.EndDate;
                    }
                    else
                    {
                        sectionRubric.StartDate = item.BeginDate.Value.AddDays((viewModel.StartDays * -1));
                        sectionRubric.EndDate = item.EndDate.Value.AddDays((viewModel.EndDays));

                    }
                    
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
                    try
                    {
                        //trying addrange instead of foreach to see if db call times are improved
                        db.SectionRubrics.AddRange(Schedules);
                    }
                    catch(Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        
                    }
                    

                    //foreach(SectionRubric sectionRubric in Schedules)
                    //{
                    //    try
                    //    {
                            
                    //        db.SectionRubrics.Add(sectionRubric);
                    //    }
                    //    catch(Exception e)
                    //    {
                    //        Debug.WriteLine(e.ToString());
                    //        Debug.WriteLine("Failed Scheduling information: \n" +
                    //            "   SectionID: " + sectionRubric.SectionID + "\n" +
                    //            "   RubricID: " + sectionRubric.RubricID + "\n" +
                    //            "   Start Date: " + sectionRubric.StartDate.ToString() + "\n" +
                    //            "   End Date: " + sectionRubric.EndDate.ToString());
                    //        continue;
                    //    }
                        
                    //}
                    db.SaveChanges();
                }
                TempData["SemesterID"] = semester.SemesterID;
                return RedirectToAction("Index", "AdminScheduling");
            }
            else
            {
                return RedirectToAction("Index", "AdminScheduling");
            }            
        }

        [HttpPost]
        public ActionResult AddRubricToCRN(SchedulingViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                SectionRubric sectionRubric = new SectionRubric
                {
                    SectionID = viewModel.SectionID,
                    RubricID = (short)viewModel.RubricID,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,

                };
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
                }
                db.SaveChanges();
                TempData["SemesterID"] = viewModel.SemesterID;
                return RedirectToAction("Index", "AdminScheduling");
                
                
            }
            else
            {
                return RedirectToAction("Index", "AdminScheduling");
            }



        }


        public ActionResult TimeframeSemester()
        {
            return View("TimeframeSemester");
        }

        public List<SelectListItem> GetSemesters(bool isScheduled)
        {

            List<SelectListItem> Semesters = new List<SelectListItem>();


            if (isScheduled)
            {
                //Couldn't get it to sort from the query, but it works this way as well
                var semesterResult = (from s in db.Semesters
                                     where s.Sections.SelectMany(sec => sec.SectionRubrics).Count() > 0                                     
                                     select new SelectListItem { Text = s.SemesterCode + " " + s.Name, Value = s.SemesterID.ToString() })
                                                 .Distinct().ToList();

                semesterResult = semesterResult.OrderByDescending(s => s.Text).ToList(); //Sorts by semester Code
                if (semesterResult.Count > 0)
                {
                    foreach (SelectListItem item in semesterResult)
                    {
                        Semesters.Add(item);
                    }
                }
                else
                {
                    Semesters.Add(new SelectListItem { Text = "No Semesters Found", Value = "0" });
                }

                return Semesters;

            }
            else
            {
                var semesterResult = (from s in db.Semesters
                                      where s.Sections.SelectMany(sec => sec.SectionRubrics).Count() < 1
                                      select new SelectListItem { Text = s.SemesterCode + " " + s.Name, Value = s.SemesterID.ToString() })
                                                 .Distinct().ToList();
                semesterResult = semesterResult.OrderByDescending(s => s.Text).ToList();
                if (semesterResult.Count > 0)
                {
                    foreach (SelectListItem item in semesterResult)
                    {
                        Semesters.Add(item);
                    }
                }
                else
                {
                    Semesters.Add(new SelectListItem { Text = "No Semesters Found", Value = "0" });
                }

                return Semesters;
            }


            
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
