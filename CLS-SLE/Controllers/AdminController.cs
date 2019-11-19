using CLS_SLE.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;


using System.Web.Mvc;
using System.Web.Routing;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
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
           
            return View();
            
        }

        public ActionResult MappingRubricCourses()
        {
            var rubricCourse = from pam in db.ProgramAssessmentMappings
                               join ar in db.AssessmentRubrics on pam.RubricID equals ar.RubricID
                               join c in db.Courses on pam.CourseID equals c.CourseID
                               select new
                               {
                                   RubricID = pam.RubricID,
                                   CourseID = c.CourseID,
                                   RubricName = ar.Name,
                                   CourseName = c.CourseName
                               };
            foreach(var rc in rubricCourse)
            {               
                Console.WriteLine(rc);
            }


            return View("AssessmentMappings", rubricCourse);
        }


        public ActionResult AssessmentMappings()
        {
            try
            {
                var personId = Convert.ToInt32(Session["personID"].ToString());
                var user = db.Users.FirstOrDefault(u => u.PersonID == personId);
                var adminAssessments = db.Assessments.ToList();
                logger.Info("Dashboard loaded for " + user?.Login);

                /* select AR.RubricID, AR.Name,
                 C.CourseID, C.CourseName

                 From ProgramAssessmentMapping PAM
                 Join AssessmentRubric AR on AR.RubricID = PAM.RubricID
                 Join Course C on C.CourseID = PAM.CourseID 
                */
                var rubricCourse = from pam in db.ProgramAssessmentMappings
                                   join ar in db.AssessmentRubrics on pam.RubricID equals ar.RubricID
                                   join c in db.Courses on pam.CourseID equals c.CourseID
                                   select new
                                   {
                                       RubricID = pam.RubricID,
                                       CourseID = c.CourseID,
                                       RubricName = ar.Name,
                                       CourseName = c.CourseName
                                   };
               
                foreach (var rc in rubricCourse)
                {
                    Console.WriteLine(rc);
                    Console.WriteLine("------------------------------");
                }



                return View(new AssessmentMappingsViewModel
                    {
                        Departments = db.Departments.ToList(),
                        Programs = db.Programs.ToList(),
                        Courses = db.Courses.ToList(),
                        Categories = db.AssessmentCategories.ToList(),
                        Assessments = adminAssessments.Distinct().OrderByDescending(a => a.IsActive).ThenBy(a => a.Name).ToList(),
                        RubricAssessments = db.RubricAssessments.ToList(),
                        AssessmentRubrics = db.AssessmentRubrics.ToList(),
                        ProgramAssessmentMappings = db.ProgramAssessmentMappings.ToList(),
                        //RubricsByProgram = db.RubricsByProgram.ToList()
                });
                

            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
 
        }

        public ActionResult Assessments()
        {
            try
            {
                var personID = Convert.ToInt32(Session["personID"].ToString());
                var user = db.Users.FirstOrDefault(u => u.PersonID == personID);
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

        public ActionResult ViewAssessment(int? assessmentId)
        {
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
                    model.CreatorLogin = (String)db.Users.Where(u => u.PersonID == assessment.CreatedByLoginID).FirstOrDefault().Login;
                }
                if (assessment.ModifiedByLoginID != null)
                {
                    model.ModifierLogin = (String)db.Users.Where(u => u.PersonID == assessment.ModifiedByLoginID).FirstOrDefault().Login;
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
        [HttpGet]
        public ActionResult AddAssessment(String category)
        {
            dynamic Model = new ExpandoObject();
            Model.Programs = (from Program in db.Programs select Program).ToList();
            Model.Category = category;
            return View(Model);
        }

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
                addAssessment.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());



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
                        editAssessment.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());
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

        /*
         * SLE-CLS Group - Fall 2019
         * Last Updated: Matt Petermann 10/7/19
         */
        public ActionResult AssessmentScheduling()
        {
            try
            {
                var personId = Convert.ToInt32(Session["personID"].ToString());
                var user = db.Users.FirstOrDefault(u => u.PersonID == personId);
                var adminAssessments = db.Assessments.ToList();
                logger.Info("Dashboard loaded for " + user?.Login);

                return View(new AssessmentSchedulingViewModel
                {
                    Departments = db.Departments.ToList(),
                    Sections = db.Sections.ToList(),
                    Semesters = db.Semesters.ToList(),
                    Programs = db.Programs.ToList(),
                    Courses = db.Courses.ToList(),
                    Categories = db.AssessmentCategories.ToList(),
                    AssessmentRubrics = db.AssessmentRubrics.ToList(),
                    ProgramAssessmentMappings = db.ProgramAssessmentMappings.ToList(),
                    Assessments = adminAssessments.Distinct().OrderByDescending(a => a.IsActive)
                        .ThenBy(a => a.Name).ToList()
                });
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        public ActionResult ProgramsCourses()
        {
            return View();
        }

        public ActionResult SchoolsDepartments()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ViewUsers(String sort)
        {

            dynamic Model = new ExpandoObject();
            var Roles = from Role in db.Roles select Role;

            Model.Roles = Roles;
            if (Request.QueryString["Search"] != null)
            {
                try
                {
                    String QueryString = Request.QueryString["Search"].ToLower();

                    if (QueryString.Equals("") || QueryString == null)
                    {
                        throw new Exception("Query is empty or null");
                    }

                    List<UserSecurity> UserSecurities = GetUserSecurities();

                    var FilteredUserSecurities = UserSecurities.Where(p => p.FirstName.ToLower().Contains(QueryString.ToLower()) || p.LastName.ToLower().Contains(QueryString.ToLower()) || p.IDNumber.Contains(QueryString.ToLower()) || p.Login.Contains(QueryString.ToLower()));

                    Model.UserSecurityList = FilteredUserSecurities;
                }
                catch
                {
                    logger.Error("Error fetching user List");
                    return Exceptions();
                }
            }

            else
            {
                Model.UserSecurityList = GetUserSecurities();
            }

            if (!String.IsNullOrEmpty(sort))
            {
                Model.Sort = sort;
            }
            return View(Model);
        }



        [HttpGet]
        public ActionResult ViewRoles()
        {
            var Roles = (from Role in db.Roles
                         select Role).OrderBy(r => r.Name);
            dynamic Model = new ExpandoObject();
            Model.Roles = Roles;

            return View(Model);
        }

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



        [HttpPost]
        public ActionResult UpdateUser(FormCollection form, String submit)
        {
            Int32 PersonID = Int32.Parse(form["personID"]);
            Int16 RoleID = RoleID = Int16.Parse(form["roleID"]);
            try
            {
                switch (submit)
                {
                    case "add":
                        UserRole userRole = new UserRole
                        {
                            PersonID = PersonID,
                            RoleID = RoleID,
                            CreatedDateTime = DateTime.Now,
                            CreatedByLoginID = (int?)Session["personID"]

                        };
                        db.UserRoles.Add(userRole);

                        break;
                    case "delete":
                        var deletionEntry = (from UserRole in db.UserRoles
                                             where UserRole.PersonID == PersonID && UserRole.RoleID == RoleID
                                             select UserRole).FirstOrDefault();
                        db.UserRoles.Remove(deletionEntry);
                        break;
                    default:
                        return Exceptions();
                }
                db.SaveChanges();
                return Content("<html><script>window.location.href = '/Admin/ManageUser?id=" + PersonID.ToString() + "';</script></html>");
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
