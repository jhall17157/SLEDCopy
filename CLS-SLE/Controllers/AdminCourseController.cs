using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    // class AdminController is extending the properties of the Controller class from System.Web.Mvc
    public class AdminCourseController : Controller
    {
        // a private int that can only be read which indicates the amount of results per page on the AdminCourse/Courses view
        private readonly int PageSize = 20;

        // creating an instance of the database context in order to access entity framework to execute database commands
        private SLE_TrackingEntities db = new SLE_TrackingEntities();

        // creating an instance of the logger that we are using to create a log of actions happening within the application
        private Logger logger = LogManager.GetCurrentClassLogger();

        // GET: AdminCourse/Courses
        /// <summary>
        ///       http get request that sends the AdminCourse/Courses view which displays a list of the courses
        /// </summary>
        /// <param name="page" >current page of courses that are being viewed</param>
        /// <returns>
        ///       a view of courses that contains a list of courses ordered by the course's Number
        /// </returns>
        public ActionResult Courses(int page, string search, string department)
        {
            CoursesViewModel coursesViewModel = new CoursesViewModel();

            int ResultsCount;
            if(department == null)
            {
                if (search == null)
                {
                    //creating a new CoursesViewModel - the Courses is a list of Courses sorted by their Number and does not include any courses with a "000-000" Number
                    //the "000-000" Numbered Courses were imported to tie some old assessment data to that was needed in the system
                    coursesViewModel.Courses = db.Courses.Where(c => c.Number != "000-000").OrderBy(c => c.Number).Skip((page - 1) * PageSize).Take(PageSize);
                    ResultsCount = db.Courses.Where(c => c.Number != "000-000").Count();
                }
                else
                {
                    coursesViewModel.Courses = db.Courses.Where(c => c.Number != "000-000").Where(c => (c.CourseName.Contains(search) || c.Number.Contains(search))).OrderBy(c => c.Number).Skip((page - 1) * PageSize).Take(PageSize);
                    ResultsCount = db.Courses.Where(c => c.Number != "000-000").Where(c => (c.CourseName.Contains(search) || c.Number.Contains(search))).Count();
                }
            }
            else
            {
                if (search == null)
                {
                    //creating a new CoursesViewModel - the Courses is a list of Courses sorted by their Number and does not include any courses with a "000-000" Number
                    //the "000-000" Numbered Courses were imported to tie some old assessment data to that was needed in the system
                    coursesViewModel.Courses = db.Courses.Where(c => c.Number != "000-000").Where(c=>c.Department.Name == department).OrderBy(c => c.Number).Skip((page - 1) * PageSize).Take(PageSize);
                    ResultsCount = db.Courses.Where(c => c.Number != "000-000").Where(c => c.Department.Name == department).Count();
                }
                else
                {
                    coursesViewModel.Courses = db.Courses.Where(c => c.Number != "000-000").Where(c => (c.CourseName.Contains(search) || c.Number.Contains(search))).Where(c => c.Department.Name == department).OrderBy(c => c.Number).Skip((page - 1) * PageSize).Take(PageSize);
                    ResultsCount = db.Courses.Where(c => c.Number != "000-000").Where(c => (c.CourseName.Contains(search) || c.Number.Contains(search))).Where(c => c.Department.Name == department).Count();
                }
            }
            
            //the Paging info is going to contain all the information required for pagination
            coursesViewModel.PagingInfo = new PagingInfo { CurrentPage = page, ItemsPerPage = PageSize, TotalItems = ResultsCount };

            List<String> departmentNames = new List<String>();

            foreach (var d in db.Departments) { departmentNames.Add(d.Name); }

            coursesViewModel.DepartmentNames = departmentNames;

            coursesViewModel.SearchInput = search;

            coursesViewModel.DepartmentFilter = department;

            return View(coursesViewModel);
        }

        [HttpPost]
        public ActionResult SearchCourse(CourseSearchViewModel searchVM) {
        
            if(db.Courses.Where(c => !c.CourseName.Contains("Folio180")).Where(c => c.CourseName.Contains(searchVM.SearchInput))!=null||
                db.Courses.Where(c => !c.CourseName.Contains("Folio180")).Where(c => c.Number.Contains(searchVM.SearchInput))!=null)
            {
                return RedirectToAction("Courses", "AdminCourse", new { page = 1, search = searchVM.SearchInput, department = searchVM.DepartmentFilter });
            }
            else { return RedirectToAction("CourseSearchError", "AdminCourse", new { search = searchVM.SearchInput }); }
        }

        public ActionResult CourseSearchError(string search) => View(new CourseSearchViewModel {SearchInput = search });


        public JsonResult CourseAutoComplete (string search)
        {
            List<CourseSearchModel> resultCourses = db.Courses.Where(c=> !c.CourseName.Contains("Folio180")).Where(c => (c.CourseName.Contains(search) || c.Number.Contains(search))).Select(c => new CourseSearchModel
            {
                id = c.CourseID,
                name = c.CourseName,
                number = c.Number,
                detailedName = c.Number + " " + c.CourseName
            }).ToList();

            return new JsonResult { Data = resultCourses, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: AdminCourse/AddCourse
        /// <summary>
        ///       http get request that sends the AdminCourse/AddCourse view which displays a form to add a new course to the database
        ///       the view adds a list of Departments to the ViewModel so that the form can contain a dropdown list of each department name
        /// </summary>
        /// <returns>
        ///       a view that contains a submission form for adding a new course
        /// </returns>
        public ActionResult AddCourse()
        {
            AddCourseViewModel courseVM = new AddCourseViewModel();

            List<String> departmentNames = new List<String>();

            foreach(var d in db.Departments) { departmentNames.Add(d.Name); }

            courseVM.DepartmentNames = departmentNames; 

            return View(courseVM);
        }

        //GET: AdminCourse/ViewCourse
        /// <summary>
        ///     http get request that sends the AdminCourse/ViewCourse view displays the details of the course specified by the param 'id'
        ///     the query is returning the first Course in the database table 'Courses' that has a CourseID of 'id'
        /// </summary>
        /// <param name="id">CourseID of the course that is being viewed</param>
        /// <returns>
        ///     a view that contains the details of the course with a CourseID matching 'id'
        /// </returns>
        public ActionResult ViewCourse(int? courseID)
        {
            var course = new Course();

            try
            {
                if (courseID.HasValue)
                {
                    course = db.Courses.FirstOrDefault(c => c.CourseID == courseID.Value);
                }

                dynamic model = new ExpandoObject();
                model.CreatorLogin = null;
                model.ModifierLogin = null;

                if (course.CreatedByLoginID != null)
                {
                    try
                    {
                        model.CreatorLogin = (String)db.Users
                            .Where(u => u.PersonID == course.CreatedByLoginID)
                            .FirstOrDefault()
                            .Login;
                    }
                    catch
                    {
                        model.CreatorLogin = "Unknown";
                    }
                }
                if (course.ModifiedByLoginID != null)
                {
                    try
                    {
                        model.ModifierLogin = (String)db.Users
                            .Where(u => u.PersonID == course.CreatedByLoginID)
                            .FirstOrDefault()
                            .Login;
                    }
                    catch
                    {
                        model.ModifierLogin = "Unknown";
                    }
                }
                model.course = course;
                model.courseSections = course.Sections.OrderByDescending(s => s.Semester.SemesterCode);
                return View(model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction("Signin", "User");
            }
        }
        // GET: AdminCourse/EditCourse
        /// <summary>
        ///     http get request that sends the AdminCourse/EditCourse view which displays a form to update information to the 
        ///     Course with the CourseID of 'id'
        /// </summary>
        /// <param name="id">the CourseID of the current Course being viewed with the ViewCourse view</param>
        /// <returns>
        ///     a view that contains a submission form for updating information to a Course
        /// </returns>
        public ActionResult EditCourse(short id) =>View(new EditCourseViewModel{Departments = db.Departments, Course = db.Courses.Where(c => c.CourseID == id).FirstOrDefault() });

        // POST: AdminCourse/CreateCourse
        /// <summary>
        ///       http post request that returns the data from the AddCourse submission form in the form of an AddCourseViewModel
        ///       than confirms that the model is valid, if valid the created date and create by fields are added and passed into the database
        /// </summary>
        /// <param name="coursetVM">A view model that contains the information of the new Course from the AddCourse submission form</param>
        /// <returns>
        ///       returns the user to the list of courses if successful otherwise returns users back to the AddCourse submission form
        /// </returns>
        [HttpPost]
        public ActionResult CreateCourse(AddCourseViewModel courseVM)
        {
            if (ModelState.IsValid)
            {
                //Adding the PrimaryDepartmentID to the courseVm
                courseVM.Course.PrimaryDepartmentID = db.Departments.Where(d => d.Name == courseVM.DepartmentSelection).FirstOrDefault().DepartmentID;
                //Adding created on date
                courseVM.Course.CreatedDateTime = DateTime.Now;
                //Adding created by
                //TODO add created by
                courseVM.Course.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                //Adding the new course to the database
                db.Courses.Add(courseVM.Course);
                db.SaveChanges();
            }
            else
            {
                //redirects user to the submission form if failed to add course
                //TODO figure out how to add form errors
                return RedirectToAction("AddCourse", "AdminCourse");
            }
            //logging that a new course was added
            logger.Info("Course id {Id} added", courseVM.Course.CourseID);
            //redirects user to the list of courses if successfully added new 
            return RedirectToAction("Courses", "AdminCourse", new { page = 1 });
        }

        //[HttpPost]
        //public ActionResult UpdateCourse(EditCourseViewModel courseVM, short id)
        //{
        //    Course editCourse = db.Courses.Where(c => c.CourseID == id).FirstOrDefault();

        //    editCourse.Name = departmentVM.Department.Name;
        //    editCourse.Number = departmentVM.Department.Number;
        //    editCourse.IsActive = departmentVM.Department.IsActive;

        //    if (editDepartment.School != departmentVM.Department.School)
        //    {
        //        School editSchool = db.Schools.Where(s => s.SchoolID == departmentVM.Department.School.SchoolID).FirstOrDefault();

        //        editSchool.Departments.Add(departmentVM.Department);
        //        editDepartment.School = departmentVM.Department.School;
        //    }

        //    editDepartment.ModifiedDateTime = DateTime.Now;
        //    editDepartment.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());

        //    db.SaveChanges();

        //    return RedirectToAction("ViewUsers", "Admin");
        //}
    }
}