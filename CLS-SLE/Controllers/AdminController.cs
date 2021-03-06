using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "AdminDashboard")]
    public class AdminController : SLEControllerBase
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdminDashboard()
        {
            ViewBag.ReportingURL = System.Configuration.ConfigurationManager.AppSettings["ReportingSiteURL"];

            return View();

        }
        //TODO: review and delete after testing new mapping page/controller
        //public ActionResult MappingRubricCourses()
        //{
        //    var rubricCourse = from pam in db.ProgramAssessmentMappings
        //                       join ar in db.AssessmentRubrics on pam.RubricID equals ar.RubricID
        //                       join c in db.Courses on pam.CourseID equals c.CourseID
        //                       select new
        //                       {
        //                           RubricID = pam.RubricID,
        //                           CourseID = c.CourseID,
        //                           RubricName = ar.Name,
        //                           CourseName = c.CourseName
        //                       };
        //    foreach (var rc in rubricCourse)
        //    {
        //        Console.WriteLine(rc);
        //    }


        //    return View("AssessmentMappings", rubricCourse);
        //}


        //public ActionResult AssessmentMappings()
        //{
        //    try
        //    {
        //        var user = db.Users.FirstOrDefault(u => u.PersonID == UserData.PersonId);
        //        var adminAssessments = db.Assessments.ToList();
        //        logger.Info("Dashboard loaded for " + user?.Login);

        //        /* select AR.RubricID, AR.Name,
        //         C.CourseID, C.CourseName

        //         From ProgramAssessmentMapping PAM
        //         Join AssessmentRubric AR on AR.RubricID = PAM.RubricID
        //         Join Course C on C.CourseID = PAM.CourseID 
        //        */
        //        var rubricCourse = from pam in db.ProgramAssessmentMappings
        //                           join ar in db.AssessmentRubrics on pam.RubricID equals ar.RubricID
        //                           join c in db.Courses on pam.CourseID equals c.CourseID
        //                           select new
        //                           {
        //                               RubricID = pam.RubricID,
        //                               CourseID = c.CourseID,
        //                               RubricName = ar.Name,
        //                               CourseName = c.CourseName
        //                           };

        //        foreach (var rc in rubricCourse)
        //        {
        //            Console.WriteLine(rc);
        //            Console.WriteLine("------------------------------");
        //        }



        //        return View(new AssessmentMappingsViewModel
        //        {
        //            Departments = db.Departments.ToList(),
        //            Programs = db.Programs.ToList(),
        //            Courses = db.Courses.ToList(),
        //            Categories = db.AssessmentCategories.ToList(),
        //            Assessments = adminAssessments.Distinct().OrderByDescending(a => a.IsActive).ThenBy(a => a.Name).ToList(),
        //            RubricAssessments = db.RubricAssessments.ToList(),
        //            AssessmentRubrics = db.AssessmentRubrics.ToList(),
        //            ProgramAssessmentMappings = db.ProgramAssessmentMappings.ToList(),
        //            //RubricsByProgram = db.RubricsByProgram.ToList()
        //        });


        //    }
        //    catch
        //    {
        //        logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
        //        return RedirectToAction(actionName: "Signin", controllerName: "User");
        //    }

        //}

        //public JsonResult getRubricsForMapping()
        //{
        //    var rubricCourse = from pam in db.ProgramAssessmentMappings
        //                       join ar in db.AssessmentRubrics on pam.RubricID equals ar.RubricID
        //                       join c in db.Courses on pam.CourseID equals c.CourseID
        //                       select new
        //                       {
        //                           RubricID = pam.RubricID,
        //                           RubricName = ar.Name,
        //                           ProgramID = pam.ProgramID
        //                       };

        //    return Json(rubricCourse.Distinct(), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetCoursesForMapping()
        //{
        //    var course = from pam in db.ProgramAssessmentMappings
        //                 join ar in db.AssessmentRubrics on pam.RubricID equals ar.RubricID
        //                 join c in db.Courses on pam.CourseID equals c.CourseID
        //                 select new
        //                 {
        //                     CourseName = c.CourseName,
        //                     CourseID = c.CourseID,
        //                     ProgramID = pam.ProgramID,
        //                     RubricID = pam.RubricID
        //                 };

        //    return Json(course.Distinct(), JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult PostRubricToMapping(String programID, String rubricID)
        //{
        //    ProgramAssessmentMapping pam = new ProgramAssessmentMapping
        //    {
        //        ProgramID = short.Parse(programID),
        //        RubricID = short.Parse(rubricID),
        //        CourseID = 110
        //    };
        //    db.ProgramAssessmentMappings.Add(pam);
        //    db.SaveChanges();
        //    return Json(pam, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult PostCourseToMapping(String programID, String rubricID, String courseID)
        //{
        //    short ProgramID = short.Parse(programID);
        //    short RubricID = short.Parse(rubricID);
        //    short CourseID = short.Parse(courseID);

        //    if (programID != null && rubricID != null && courseID != null)
        //    {
        //        var pam = db.ProgramAssessmentMappings.FirstOrDefault(p => p.ProgramID == ProgramID && p.RubricID == RubricID);

        //        if (pam.CourseID == 110)
        //        {
        //            pam.CourseID = CourseID;
        //        }
        //        else
        //        {
        //            ProgramAssessmentMapping p = new ProgramAssessmentMapping
        //            {
        //                ProgramID = ProgramID,
        //                RubricID = RubricID,
        //                CourseID = CourseID
        //            };
        //            db.ProgramAssessmentMappings.Add(p);
        //        }
        //    }
        //    db.SaveChanges();
        //    return Json(JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult DeleteRubricFromMapping(String programID, String rubricID)
        //{
        //    short ProgramID = short.Parse(programID);
        //    short RubricID = short.Parse(rubricID);
        //    var pams = db.ProgramAssessmentMappings;
        //    if (programID != null && rubricID != null)
        //    {
        //        var pam = pams.Where(p => p.ProgramID == ProgramID && p.RubricID == RubricID);
        //        foreach (var p in pam)
        //        {
        //            db.ProgramAssessmentMappings.Remove(p);
        //        }
        //        db.SaveChanges();
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public void UpdateDateRange(string startDateTime, string endDateTime, string assessmentRubricId)
        {
            var id = short.Parse(assessmentRubricId);
            var assessmentRubric = db.RubricAssessments.FirstOrDefault(r => r.RubricAssessmentID == id);

            if (assessmentRubric != null)
            {
                if (startDateTime != "")
                {
                    assessmentRubric.StartDate = DateTime.Parse(startDateTime);
                }

                if (endDateTime != "")
                {
                    assessmentRubric.EndDate = DateTime.Parse(endDateTime);
                }
            }

            db.SaveChanges();
        }

        public JsonResult DeleteCourseFromRubric(String programID, String rubricID, String courseID)
        {
            short ProgramID = short.Parse(programID);
            short RubricID = short.Parse(rubricID);
            short CourseID = short.Parse(courseID);

            var pams = db.ProgramAssessmentMappings;
            if (programID != null && rubricID != null)
            {
                var pam = pams.Where(p => p.ProgramID == ProgramID && p.RubricID == RubricID && p.CourseID == CourseID);
                foreach (var p in pam)
                {
                    db.ProgramAssessmentMappings.Remove(p);
                }
                db.SaveChanges();
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles ="Assessments")]
        public ActionResult Assessments()
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.PersonID == UserData.PersonId);
                var adminAssessments = from assessments in db.Assessments
                                           //join permissions in db.AssessmentRubricSecurities on assessments.AssessmentID equals permissions.AssessmentID
                                           //where permissions.PersonID == personID
                                       select assessments;
                logger.Info("Dashboard loaded for " + user.Login);
                var categories = db.AssessmentCategories.ToList();

                dynamic model = new ExpandoObject();

                model.assessments = adminAssessments.Distinct().OrderByDescending(a => a.IsActive).ThenBy(a => a.Name).ToList();
                model.categories = categories;

                return View(model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [Authorize(Roles = "ViewAssessments")]
        public ActionResult ViewAssessment(int? assessmentId)
        {
            String DefaultUserCreateModifyValue = "Unknown";
            var assessment = new Assessment();
            var canEdit = false;
            var canAdd = false;
            try
            {
                if (assessmentId.HasValue)
                {
                    assessment = db.Assessments.FirstOrDefault(a => a.AssessmentID == assessmentId.Value);
                    var permission = db.AssessmentRubricSecurities.FirstOrDefault(p => p.AssessmentID == assessmentId.Value);
                    if (permission != null)
                    {
                        canEdit = permission.CanEdit == 1 ? true : false;
                    }
                }

                if (assessmentId.HasValue)
                {
                    var permission = db.AssessmentRubricSecurities.FirstOrDefault(p => p.AssessmentID == assessmentId.Value);
                    if (permission != null)
                    {
                        canAdd = permission.CanEdit == 1 ? true : false;
                    }
                }

                dynamic model = new ExpandoObject();
                model.CreatorLogin = null;
                model.ModifierLogin = null;

                if (assessment.CreatedByLoginID != null)
                {
                    var tempVar = db.Users.Where(u => u.PersonID == assessment.CreatedByLoginID).FirstOrDefault();
                    if(tempVar!=null)
                    {
                        model.CreatorLogin = (String)db.Users.Where(u => u.PersonID == assessment.CreatedByLoginID).FirstOrDefault().Login;
                    }
                    else
                    {
                        model.CreatorLogin = DefaultUserCreateModifyValue;
                    }
                }
                if (assessment.ModifiedByLoginID != null)
                {
                    var tempVar = db.Users.Where(u => u.PersonID == assessment.ModifiedByLoginID).FirstOrDefault();
                    if(tempVar != null)
                    {
                        model.ModifierLogin = (String)db.Users.Where(u => u.PersonID == assessment.ModifiedByLoginID).FirstOrDefault().Login;
                    }
                    else
                    {
                        model.ModifierLogin = DefaultUserCreateModifyValue;
                    }
                }
                model.program = db.Programs.Where(p => p.ProgramID == assessment.ProgramID).FirstOrDefault().Name;
                model.assessment = assessment;
                model.canEdit = canEdit;
                model.canAdd = canAdd;

                return View(model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [Authorize(Roles = "EditAssessments")]
        public ActionResult EditAssessment(int? assessmentId)
        {
            var assessment = new Assessment();

            dynamic Model = new ExpandoObject();


            try
            {
                if (assessmentId.HasValue)
                {
                    //var canEdit = false;
                    assessment = db.Assessments.FirstOrDefault(a => a.AssessmentID == assessmentId.Value);
                    //var permission = db.AssessmentRubricSecurities.FirstOrDefault(p => p.AssessmentID == assessmentId.Value);

                    //if (permission != null)
                    //{
                    //    canEdit = permission.CanEdit == 1 ? true : false;
                    //}

                    //if (!canEdit)
                    //{
                    //    throw new Exception("User cannot edit this assessment");
                    //}
                }
                else
                {
                    throw new Exception("Assessment not found!");
                }
                Model.Programs = (from Program in db.Programs select Program).ToList();
                Model.Assessment = assessment;
                return View(Model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return Exceptions();
            }
        }

        [Authorize(Roles = "AddRubrics")]
        public ActionResult AddRubricToCourse(int? rubricI)
        {
            try
            {

            }
            catch
            {
                return View("AssessmentMappings");
            }
            return View("AssessmentMappings");
        }
        //[HttpGet]
        //public ActionResult ViewUsers(ViewUserViewModel viewUserViewModel)
        //{

        [Authorize(Roles = "AddAssessments")]
        [HttpPost]
        public ActionResult InsertNewAssessment(FormCollection formCollection)
        {
            try
            {
                db.Assessments.Load();
                string Category = formCollection["Category"];
                var CategoryCode = db.AssessmentCategories.Where(c => c.Name == Category).FirstOrDefault().CategoryCode;
                var program = (formCollection["Program"]);
                Assessment addAssessment = db.Assessments.Create();


                addAssessment.Name = formCollection["Name"];
                addAssessment.Category = CategoryCode;
                addAssessment.Description = formCollection["Description"];
                addAssessment.OutcomePassRate = (Decimal?)(Double.Parse(formCollection["PassPercent"])) / 100;
                addAssessment.CalculateOutcomePassRate = ((formCollection["CalculateOutcomePassRate"]).Equals("True") ? true : false);
                addAssessment.ProgramID = db.Programs.Where(p => p.Name == program).FirstOrDefault().ProgramID;
                addAssessment.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                addAssessment.CreatedDateTime = DateTime.Now;
                addAssessment.CreatedByLoginID = UserData.PersonId;



                db.Entry(addAssessment).State = EntityState.Added;
                db.SaveChanges();


                return RedirectToAction(actionName: "Assessments", controllerName: "Admin");

            }
            catch
            {
                logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [Authorize(Roles = "EditAssessments")]
        [HttpPost]
        public ActionResult SaveAssessment(FormCollection formCollection)
        {
            try
            {
                if (Int32.Parse(formCollection["AssessmentID"]) > 0)
                {
                    var assessmentid = Int32.Parse(formCollection["AssessmentID"]);
                    var editAssessment = db.Assessments.FirstOrDefault(a => a.AssessmentID == assessmentid);

                    if (editAssessment != null)
                    {
                        editAssessment.Name = formCollection["Name"];
                        editAssessment.Category = formCollection["Category"];
                        editAssessment.Description = formCollection["Description"];
                        editAssessment.OutcomePassRate = (Decimal?)(Double.Parse(formCollection["PassPercent"])) / 100;
                        editAssessment.CalculateOutcomePassRate = ((formCollection["CalculateOutcomePassRate"]).Equals("True") ? true : false);
                        var program = (formCollection["Program"]);
                        editAssessment.ProgramID = db.Programs.Where(p => p.Name == program).FirstOrDefault().ProgramID;
                        editAssessment.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                        editAssessment.ModifiedDateTime = DateTime.Now;
                        editAssessment.ModifiedByLoginID = UserData.PersonId;
                        db.SaveChanges();

                        return RedirectToAction(actionName: "ViewAssessment", controllerName: "Admin", routeValues: new { assessmentId = editAssessment.AssessmentID });
                    }
                    else
                    {
                        logger.Error("Failed to save assessment, redirecting to sign in page.");
                        return RedirectToAction(actionName: "Signin", controllerName: "User");
                    }
                }
                else
                {
                    logger.Error("Failed to save assessment, redirecting to sign in page.");
                    return RedirectToAction(actionName: "Signin", controllerName: "User");
                }
            }
            catch
            {
                logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        //TODO: review and remove after creating new scheduling page/controller
        ///*
        // * SLE-CLS Group - Fall 2019
        // * Last Updated: Matt Petermann 10/7/19
        // */
        //public ActionResult AssessmentScheduling()
        //{
        //    try
        //    {
        //        var user = db.Users.FirstOrDefault(u => u.PersonID == UserData.PersonId);
        //        var adminAssessments = db.Assessments.ToList();
        //        logger.Info("Dashboard loaded for " + user?.Login);

        //        return View(new AssessmentSchedulingViewModel
        //        {
        //            Departments = db.Departments.ToList(),
        //            Sections = db.Sections.ToList(),
        //            Semesters = db.Semesters.ToList(),
        //            Programs = db.Programs.ToList(),
        //            Courses = db.Courses.ToList(),
        //            Categories = db.AssessmentCategories.ToList(),
        //            AssessmentRubrics = db.AssessmentRubrics.ToList(),
        //            ProgramAssessmentMappings = db.ProgramAssessmentMappings.ToList(),
        //            Assessments = adminAssessments.Distinct().OrderByDescending(a => a.IsActive)
        //                .ThenBy(a => a.Name).ToList()
        //        });
        //    }
        //    catch
        //    {
        //        logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
        //        return RedirectToAction(actionName: "Signin", controllerName: "User");
        //    }
        //}

        [Authorize(Roles = "ProgramsCourses")]
        public ActionResult ProgramsCourses()
        {
            return View();
        }

        [Authorize(Roles = "ViewSchools")]
        public ActionResult SchoolsDepartments()
        {
            return View();
        }

        [Authorize(Roles = "ProgramsCourses")]
        public ActionResult ProgramDashboard() => View();

        [Authorize(Roles = "Users")]
        [HttpGet]
        public ActionResult ViewUsers(ViewUserViewModel viewUserViewModel)
        {

            dynamic Model = new ExpandoObject();
            var Roles = from Role in db.Roles select Role;

            viewUserViewModel.Roles = from Role in db.Roles select Role;
            if (Request.QueryString["Search"] != null)
            {
                try
                {
                    String QueryString = Request.QueryString["Search"].ToLower();

                    if (QueryString.Equals("") || QueryString == null)
                    {
                        throw new Exception("Query is empty or null");
                    }

                    viewUserViewModel.UserSecurities = GetUserSecurities();

                    var FilteredUserSecurities = viewUserViewModel.UserSecurities.Where(p => p.FirstName.ToLower().Contains(QueryString.ToLower()) || p.LastName.ToLower().Contains(QueryString.ToLower()) || p.IDNumber.Contains(QueryString.ToLower()) || p.Login.Contains(QueryString.ToLower()));

                    viewUserViewModel.UserSecurities = (List<UserSecurity>)FilteredUserSecurities;
                }
                catch
                {
                    logger.Error("Error fetching user List");
                    return Exceptions();
                }
            }

            else
            {
                viewUserViewModel.UserSecurities = GetUserSecurities();
            }

            /*if (!String.IsNullOrEmpty(sort))
            {
                //Model.Sort = sort;
            }*/
            return View(viewUserViewModel);
        }


        [Authorize(Roles = "Roles")]
        [HttpGet]
        public ActionResult ViewRoles()
        {
            var Roles = (from Role in db.Roles
                         select Role).OrderBy(r => r.Name);
            dynamic Model = new ExpandoObject();
            Model.Roles = Roles;

            return View(Model);
        }

        [Authorize(Roles = "Roles")]
        [HttpGet]
        public ActionResult ViewRoleMembers(Int16 roleID)
        {
            var CurrentRole = (from Role in db.Roles
                               where Role.RoleID == roleID
                               select Role).FirstOrDefault();
            var UserSecurityList = GetUserSecurities().Where(p => p.Roles.Any(r => r.RoleID == roleID));
            dynamic Model = new ExpandoObject();
            Model.UserSecurityList = UserSecurityList;
            Model.CurrentRole = CurrentRole;
            return View(Model);

        }


        [Authorize(Roles = "CreateManageUsers")]
        [HttpGet]
        public ActionResult ManageUser(int id)
        {
            try
            {
                int UserID = id;
                var Roles = (from role in db.Roles
                             select role).OrderBy(r => r.Name);
                var UserRoles = from userRole in db.UserRoles
                                where userRole.PersonID == UserID
                                select userRole;
                var User = (from user in db.Users
                            join person in db.People on user.PersonID equals person.PersonID
                            where user.PersonID == UserID
                            select new { user.PersonID, user.Login, person.FirstName, person.LastName, person.IdNumber }).FirstOrDefault();

                ManageUser Model = new ManageUser(User.PersonID, User.Login, User.IdNumber, User.FirstName, User.LastName, Roles.ToList(), UserRoles.ToList());


                return View(Model);
            }
            catch
            {
                return Exceptions();
            }
        }


        [Authorize(Roles = "CreateManageUsers")]
        [HttpPost]
        public ActionResult UpdateUser(FormCollection form, String submit, short personID, short roleID)
        {
            // Int32 PersonID = Int32.Parse(form["personID"]);
            // Int16 RoleID = RoleID = Int16.Parse(form["roleID"]);
            try
            {
                switch (submit)
                {
                    case "add":
                        UserRole userRole = new UserRole
                        {
                            PersonID = personID,
                            RoleID = roleID,
                            CreatedDateTime = DateTime.Now,
                            CreatedByLoginID = UserData.PersonId

                        };
                        db.UserRoles.Add(userRole);

                        break;
                    case "delete":
                        var deletionEntry = (from UserRole in db.UserRoles
                                             where UserRole.PersonID == personID && UserRole.RoleID == roleID
                                             select UserRole).FirstOrDefault();
                        db.UserRoles.Remove(deletionEntry);
                        break;
                    default:
                        return Exceptions();
                }
                db.SaveChanges();
                return RedirectToAction("ManageUser", "Admin", new { id = personID });
                // return Content("<html><script>window.location.href = '/Admin/ManageUser?id=" + PersonID.ToString() + "';</script></html>");
            }
            catch
            {
                return Exceptions();
            }
        }


        private ActionResult Exceptions()
        {
            return RedirectToAction(actionName: "AdminDashboard", controllerName: "Admin");
        }


        private List<UserSecurity> GetUserSecurities()
        {

            var Users = (from user in db.Users
                         join person in db.People on user.PersonID equals person.PersonID
                         select new { FirstName = person.FirstName, Login = user.Login, LastName = person.LastName, PersonID = person.PersonID, IDNumber = person.IdNumber, User = user }).OrderBy(p => p.Login);

            var UserRoles = (from role in db.Roles
                             join userRole in db.UserRoles on role.RoleID equals userRole.RoleID
                             join user in db.Users on userRole.PersonID equals user.PersonID
                             select new { PersonID = user.PersonID, RoleName = role.Name, RoleID = role.RoleID }); ;

            var UserList = Users.ToList();

            var UserRoleList = UserRoles.ToList();
            var UserSecurityList = new List<UserSecurity>();
            foreach (var user in UserList)
            {
                var userRoles = new List<Role>();
                foreach (var userRole in UserRoleList)
                {
                    if (userRole.PersonID.Equals(user.PersonID))
                    {
                        Role role = new Role();
                        role.RoleID = userRole.RoleID;
                        role.Name = userRole.RoleName;
                        userRoles.Add(role);

                    }
                }
                UserSecurityList.Add(new UserSecurity(user.PersonID, user.Login, user.IDNumber, user.FirstName, user.LastName, userRoles, user.User));
            }
            return UserSecurityList;

        }
    }
}