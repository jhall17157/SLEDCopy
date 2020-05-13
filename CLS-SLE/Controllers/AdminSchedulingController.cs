﻿using System;
using System.Collections.Generic;
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



            //This is the only way I could get the sorting to work properly. Orderby in the query didn't seem to want to work at all. Sorting the 
            //results directly in the model's list didn't work. Reverse didn't work.
            //Only option was to create a temp variable, sort that back onto itself, then add all the items to the model. Inelegant but it works.
            //var semesterResult = (from s in db.Semesters
            //                      join sec in db.Sections on s.SemesterID equals sec.SemesterID                                  
            //                      where sec.SectionRubrics.Count > 0
            //                      select new SelectListItem { Text = s.SemesterCode + " " + s.Name, Value = s.SemesterID.ToString() })
            //                                 .Distinct().ToList();
            //semesterResult = semesterResult.OrderByDescending(s => s.Value).ToList();
            //schedulingViewModel.Semesters = new List<SelectListItem>();
            //foreach(SelectListItem item in semesterResult)
            //{
            //    schedulingViewModel.Semesters.Add(item);
            //}
            schedulingViewModel.Semesters = GetSemesters(true);

            if (TempData["SemesterID"] != null)
            {
                schedulingViewModel.SemesterID = (int)TempData["ProgramID"];
            }





            return View(schedulingViewModel);
        }
        //Called when user selects a semester
        //[HttpPost]
        //public ActionResult Index(SchedulingViewModel viewModel)
        //{
        //    SchedulingViewModel schedulingViewModel = new SchedulingViewModel();
        //    //schedulingViewModel.People = db.People;
        //    schedulingViewModel.Semesters = (from s in db.Semesters
        //                                     select new SelectListItem { Text = s.SemesterCode + " " + s.Name, Value = s.SemesterID.ToString() })
        //        .Distinct().ToList();
        //    schedulingViewModel.Semester =
        //        db.Semesters.FirstOrDefault(s => s.SemesterID == viewModel.SemesterID);
        //    //get course ids for all sections in selected semester
        //    List<short> courseIDs = schedulingViewModel.Semester.Sections.Select(i => i.CourseID).ToList();

        //    schedulingViewModel.Courses = db.Courses.Where(c => courseIDs.Contains(c.CourseID)).ToList();

        //    return View(schedulingViewModel);
        //}


        [HttpPost]
        public ActionResult Index(SchedulingViewModel viewModel, int page = 1)
        {
            

            SchedulingViewModel schedulingViewModel = new SchedulingViewModel();


            if (TempData["SemesterID"] != null)
            {
                schedulingViewModel.SemesterID = (int)TempData["ProgramID"];
            }
            else
            {
                schedulingViewModel.SemesterID = viewModel.SemesterID;
            }            
            schedulingViewModel.Semester = db.Semesters.FirstOrDefault(s => s.SemesterID == schedulingViewModel.SemesterID);

            //sorted descending
            //var semesterResult = (from s in db.Semesters
            //                      join sec in db.Sections on s.SemesterID equals sec.SemesterID
            //                      where sec.SectionRubrics.Count > 0
            //                      select new SelectListItem { Text = s.SemesterCode + " " + s.Name, Value = s.SemesterID.ToString() })
            //                                 .Distinct().ToList();
            //semesterResult = semesterResult.OrderByDescending(s => s.Value).ToList();
            //schedulingViewModel.Semesters = new List<SelectListItem>();
            //foreach (SelectListItem item in semesterResult)
            //{
            //    schedulingViewModel.Semesters.Add(item);
            //}
            //schedulingViewModel.Semesters = (from s in db.Semesters
            //                                 orderby s.SemesterID descending
            //                                 select new SelectListItem
            //                                 {
            //                                     Text = s.SemesterCode + " " + s.Name,
            //                                     Value = s.SemesterID.ToString()
            //                                 })
            //                                 .Distinct().ToList();
            schedulingViewModel.Semesters = GetSemesters(true);
            
            //get course ids for all sections in selected semester and returns as a list 
            List<short> courseIDs = schedulingViewModel.Semester.Sections.Select(i => i.CourseID).ToList();
            //handles pagination
            schedulingViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = db.Courses
                .Where(c => courseIDs.Contains(c.CourseID)).Count()
            };
            //get all courses whose courseIDs exist in provided list of courseids
            var courseResult = db.Courses
                .Where(c => courseIDs.Contains(c.CourseID));
                //.OrderBy(c => c.CourseName).ToList();
            if(viewModel.CourseID != null && viewModel.CourseID != -1)
            {
                courseResult = courseResult.Where(c => c.CourseID == viewModel.CourseID);
            }
            schedulingViewModel.Courses = courseResult.OrderBy(c => c.CourseName).ToList();
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
                                 m.RubricID,                                 
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
                TempData["SemesterID"] = semester.SemesterID;
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




                //var semesterResult = (from s in db.Semesters
                //                      join sec in db.Sections on s.SemesterID equals sec.SemesterID
                //                      where sec.SectionRubrics.Count > 0
                //                      select new SelectListItem { Text = s.SemesterCode + " " + s.Name, Value = s.SemesterID.ToString() })
                //                                 .Distinct().ToList();
                semesterResult = semesterResult.OrderByDescending(s => s.Value).ToList();
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
                semesterResult = semesterResult.OrderByDescending(s => s.Value).ToList();
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
