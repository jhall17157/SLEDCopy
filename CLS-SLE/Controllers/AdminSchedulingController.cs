using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Scheduling")]
    public class AdminSchedulingController : SLEControllerBase
    {

        private SLE_TrackingEntities db = new SLE_TrackingEntities();

        private readonly int pageSize = 10;


        // GET: Admin Scheduling home page with list of semesters to choose from.
        [Authorize(Roles = "Scheduling")]
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

        //Called when User Selects a Semester or Initiates a Search
        [Authorize(Roles = "Scheduling")]
        [HttpPost]
        public ActionResult Index(SchedulingViewModel viewModel, int page = 1)
        {
            SchedulingViewModel schedulingViewModel = new SchedulingViewModel();
            schedulingViewModel.Sections = Enumerable.Empty<SelectListItem>().ToList();
            string focusArea = null;

            //Defining Rubrics
            schedulingViewModel.AssesmentRubrics = (from r in db.AssessmentRubrics
                                                    select new SelectListItem { Text = r.Name, Value = r.RubricID.ToString() }).Distinct().ToList(); //2ms

            if (TempData["SemesterID"] != null) //44ms
            {
                schedulingViewModel.SemesterID = (int)TempData["SemesterID"];
            }
            else
            {
                schedulingViewModel.SemesterID = viewModel.SemesterID;
            }
            schedulingViewModel.Semester = db.Semesters.FirstOrDefault(s => s.SemesterID == schedulingViewModel.SemesterID);

            schedulingViewModel.Semesters = GetSemesters(true);//53ms

            //get course ids for all sections in selected semester that have assessments scheduled and returns as a list 
            // exclude cancelled courses
            List<short> courseIDs = db.Sections
                .Where(c => c.SectionRubrics.Any() && c.SemesterID == schedulingViewModel.Semester.SemesterID && c.IsCancelled == false)
                .Select(i => i.CourseID).Distinct().ToList();

            //schedulingViewModel.Semester.Sections.Where(c => c.SectionRubrics.Any()).Select(i => i.CourseID).Distinct().ToList(); //23ms

            //handles pagination

            schedulingViewModel.PagingInfo = new ViewModels.PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageSize,
                TotalItems = courseIDs.Count()

            };
            //
            //get all courses whose courseIDs exist in provided list of courseids
            var courseResult = db.Courses
                .Where(c => courseIDs.Contains(c.CourseID));

            if (viewModel.searchTerm != null && !viewModel.searchTerm.Equals(""))//13 ms
            {
                int searchTermCRN = -1;
                bool searchTermIsCRN = false;
                if(viewModel.searchTerm.Length >=4 && viewModel.searchTerm.Length <=5)
                {
                    searchTermIsCRN = int.TryParse(viewModel.searchTerm, out searchTermCRN);
                }
                if(searchTermIsCRN)
                {
                    courseResult = courseResult.Where(c => c.Sections.Where(s => s.CRN.ToString().Contains(viewModel.searchTerm) && s.SemesterID == schedulingViewModel.SemesterID).Any());
                    if(courseResult.Any() && courseResult.Count() == 1)
                    {
                        focusArea = searchTermCRN.ToString();
                        viewModel.FocusIDCRN = focusArea;
                        ViewBag.FocusIDCRN = focusArea;
                        viewModel.FocusIDCourse =  courseResult.FirstOrDefault().Number;
                        ViewBag.FocusIDCourse = courseResult.FirstOrDefault().Number;
                    }
                }
                else
                {
                    courseResult = courseResult.Where(c => (c.CourseName.Contains(viewModel.searchTerm) || c.Number.ToString().Contains(viewModel.searchTerm)));
                }
                //courseResult = courseResult.Where(c => ( c.CourseName.Contains(viewModel.searchTerm)||c.Number.ToString().Contains(viewModel.searchTerm)||c.Sections.Where(s => s.CRN.ToString().Contains(viewModel.searchTerm) && s.SemesterID == schedulingViewModel.SemesterID).Any()));

            }
            schedulingViewModel.Courses = courseResult.OrderBy(c => c.Number).ToList();
            schedulingViewModel.CourseSelectList = new List<SelectListItem>();//60 ms
            schedulingViewModel.CourseSelectList.Add(new SelectListItem { Text = "All Courses For Semester", Value = "-1" });

            schedulingViewModel.CourseSelectList = (from c in schedulingViewModel.Courses
                                                    select new SelectListItem { Text = c.Number + " " + c.CourseName, Value = c.CourseID.ToString() }).Distinct().ToList();


            schedulingViewModel.CourseForAddEntry = (from c in db.Courses
                                                     where c.Sections.Where(y => y.SemesterID == schedulingViewModel.SemesterID && y.IsCancelled == false).Any()
                                                     orderby c.Number
                                                     select new SelectListItem { Text = c.Number + " " + c.CourseName, Value = c.CourseID.ToString() }).Distinct().ToList();

            //foreach (Course course in schedulingViewModel.Courses)
            //{
            //    schedulingViewModel.CourseSelectList.Add(new SelectListItem { Text = course.Number + " " + course.CourseName, Value = course.CourseID.ToString() });
            //}

            schedulingViewModel.Courses = schedulingViewModel.Courses
                .Skip((schedulingViewModel.PagingInfo.CurrentPage - 1) * schedulingViewModel.PagingInfo.ItemsPerPage)
                .Take(schedulingViewModel.PagingInfo.ItemsPerPage).ToList();

            ModelState.Clear();
            return View(schedulingViewModel);
        }

        [Authorize(Roles = "ScheduleSemester")]
        public ActionResult ScheduleSemester()
        {
            ScheduleSemesterViewModel scheduleSemesterViewModel = new ScheduleSemesterViewModel();

            scheduleSemesterViewModel.Semesters = GetSemesters(false);

            return View(scheduleSemesterViewModel);
        }

        [Authorize(Roles = "AddNewScheduleEntry")]
        [HttpPost]
        public ActionResult CreateSemesterSchedule(ScheduleSemesterViewModel viewModel)
        {
            //rather than write an entire additional class for conditional validation this is simpler
            if (!viewModel.isDates)
            {
                ModelState["StartDate"].Errors.Clear();
                ModelState["EndDate"].Errors.Clear();
            }
            //check model state before continuing
            if (ModelState.IsValid)
            {


                ////pull semester info and default start and end dates from view model
                //Semester semester = viewModel.Semester;


                //based on selected semester, grab all rubrics from mappings and sections from sections tables
                var result = from m in db.ProgramAssessmentMappings
                             join s in db.Sections on m.CourseID equals s.CourseID
                             where s.SemesterID == viewModel.SemesterID
                             select new
                             {
                                 s.SectionID,
                                 s.BeginDate,
                                 s.EndDate,
                                 m.RubricID,
                             };

                //ensure results are distinct to avoid duplicate entries
                if (result.Any())
                {
                    result = result.Distinct();
                }


                //build list of schedule entries from result set
                List<SectionRubric> Schedules = new List<SectionRubric>();
                //check if result is empty
                if (result.Any() && result != null)
                {
                    //Iterate through result set
                    foreach (var item in result)
                    {
                        SectionRubric sectionRubric = new SectionRubric();
                        sectionRubric.SectionID = item.SectionID;
                        sectionRubric.RubricID = item.RubricID;
                        //check if dates are set by specific date or based on section end date
                        if (viewModel.isDates)
                        {
                            sectionRubric.StartDate = viewModel.StartDate;
                            sectionRubric.EndDate = viewModel.EndDate;
                        }
                        else
                        {
                            sectionRubric.StartDate = item.EndDate.Value.AddDays((viewModel.StartDays.Value * -1));
                            sectionRubric.EndDate = item.EndDate.Value.AddDays((viewModel.EndDays.Value));

                        }

                        sectionRubric.CreatedDateTime = DateTime.Now;
                        sectionRubric.CreatedByLoginID = UserData.PersonId;

                        Schedules.Add(sectionRubric);
                    }
                }

                //Check if new schedule list is empty
                if (Schedules.Any())
                {
                    try
                    {
                        //trying addrange instead of foreach to see if db call times are improved
                        db.SectionRubrics.AddRange(Schedules);
                    }
                    catch (Exception e)
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
                TempData["SemesterID"] = viewModel.SemesterID;
                return RedirectToAction("Index", "AdminScheduling");
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return RedirectToAction("Index", "AdminScheduling");
            }
        }

        
        [HttpGet]
        [Route("/Controllers/AdminScheduling/GetSections/{semesterID, courseID}")]
        public ActionResult GetSections(int semesterID, int courseID)
        {
            var sectionResult = db.Sections.Where(s => s.CourseID == courseID && s.SemesterID == semesterID).Select(s => new
            {
                Text = s.CRN,
                Value = s.SectionID,
                //Text = s.CRN
            }).Distinct().OrderBy(s => s.Text).ToList();
            foreach (var item in sectionResult)
            {
                Debug.WriteLine(item.Value + " " + item.Text);
            }


            return Json(sectionResult, JsonRequestBehavior.AllowGet);
        }
        //TODO: Get SectionRubrics

        [Authorize(Roles = "Scheduling")]
        [HttpPost]
        public ActionResult AddRubricToCRN(SchedulingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //start upload loop
                foreach (var CRN in viewModel.SectionIDs) //in the viewmodel we have a list of CRNs called "sectionIDs" which is a reference back to the older single CRN upload method's sectionID
                {
                    SectionRubric sectionRubric = new SectionRubric
                    {
                        SectionID = CRN, //The SectionID represents the course CRN selected
                        RubricID = (short)viewModel.RubricID,
                        StartDate = viewModel.StartDate,
                        EndDate = viewModel.EndDate,
                        CreatedByLoginID = UserData.PersonId,
                        CreatedDateTime = DateTime.Now


                    };
                    try
                    {
                        db.SectionRubrics.Add(sectionRubric);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        Debug.WriteLine("Failed Scheduling information: \n" +
                            "   SectionID: " + sectionRubric.SectionID + "\n" +
                            "   RubricID: " + sectionRubric.RubricID + "\n" +
                            "   Start Date: " + sectionRubric.StartDate.ToString() + "\n" +
                            "   End Date: " + sectionRubric.EndDate.ToString());
                    }
                    db.SaveChanges();
                }              
                //end upload loop
                TempData["SemesterID"] = viewModel.SemesterID;
                return RedirectToAction("Index", "AdminScheduling");


            }
            else
            {
                return RedirectToAction("Index", "AdminScheduling");
            }



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

        [Authorize(Roles = "EditAssessmentSchedules")]
        [HttpPost]
        public ActionResult DeleteSchedule(int id, int sId)
        {

            //SchedulingViewModel svm = new SchedulingViewModel
            //{
            //    SemesterID = sId
            //};

            TempData["SemesterID"] = sId;
            if (ModelState.IsValid)
            {
                var record = db.SectionRubrics.SingleOrDefault(s => s.SectionRubricID == id);
                try
                {
                    db.SectionRubrics.Remove(record);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error");
                    return RedirectToAction("Index", "AdminScheduling");
                }
            }
            return RedirectToAction("Index", "AdminScheduling");
        }




        [Authorize(Roles = "Scheduling")]
        [HttpPost]
        public ActionResult SaveSchedule(SchedulingViewModel svm, int id, int sId)
        {


            TempData["SemesterID"] = sId;
            if (ModelState.IsValid)
            {
                var record = db.SectionRubrics.SingleOrDefault(s => s.SectionRubricID == id);
                if (record != null)
                {
                    record.StartDate = svm.StartDate;
                    record.EndDate = svm.EndDate;
                    record.ModifiedByLoginID = UserData.PersonId;
                    record.ModifiedDateTime = DateTime.Now;
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Index", "AdminScheduling");
                }
            }

            return RedirectToAction("Index", "AdminScheduling");
        }

        public JsonResult SaveDateChange(int SectionRubricID, DateTime beginDate, DateTime endDate)
        {
            SectionRubric sectionRubric = db.SectionRubrics.Where(sr => sr.SectionRubricID == SectionRubricID).FirstOrDefault();
            try
            {
                if (sectionRubric != null)
                {
                    sectionRubric.StartDate = beginDate;
                    sectionRubric.EndDate = endDate;
                    sectionRubric.ModifiedByLoginID = UserData.PersonId;
                    sectionRubric.ModifiedDateTime = DateTime.Now;
                    db.SaveChanges();
                    return new JsonResult { Data = SectionRubricID, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                return new JsonResult { Data = e, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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
