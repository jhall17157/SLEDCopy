using CLS_SLE.Models;
using NLog;
using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
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

        public ActionResult AssessmentMappings()
        {
            return View();
        }

        public ActionResult Assessments()
        {
            try
            {
                var personID = Convert.ToInt32(Session["personID"].ToString());
                var user = db.Users.FirstOrDefault(u => u.PersonID == personID);
                var adminAssessments = from assessments in db.Assessments
                                       join permissions in db.AssessmentRubricSecurities on assessments.AssessmentID equals permissions.AssessmentID
                                       where permissions.PersonID == personID
                                       select assessments;
                logger.Info("Dashboard loaded for " + user.Login);
                var categories = db.AssessmentCategories.ToList();

                dynamic model = new ExpandoObject();

                model.assessments = adminAssessments.Distinct().ToList();
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

            try
            {
                if (assessmentId.HasValue)
                {
                    var canEdit = false;
                    assessment = db.Assessments.FirstOrDefault(a => a.AssessmentID == assessmentId.Value);
                    var permission = db.AssessmentRubricSecurities.FirstOrDefault(p => p.AssessmentID == assessmentId.Value);

                    if (permission != null)
                    {
                        canEdit = permission.CanEdit == 1 ? true : false;
                    }

                    if (!canEdit)
                    {
                        throw new Exception("User cannot edit this assessment");
                    }
                }
                else
                {
                    throw new Exception("Assessment not found!");
                }

                return View(assessment);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        public ActionResult AddAssessment(Assessment assessment)
        {
            var addAssessment = new Assessment();

            try
            {
                if (addAssessment != null)
                {
                    addAssessment.AssessmentID = assessment.AssessmentID;
                    addAssessment.Name = assessment.Name;
                    addAssessment.Category = assessment.Category;
                    addAssessment.Description = assessment.Description;
                    addAssessment.OutcomePassRate = assessment.OutcomePassRate;
                    addAssessment.CalculateOutcomePassRate = assessment.CalculateOutcomePassRate;
                    addAssessment.ProgramID = assessment.ProgramID;
                    addAssessment.IsActive = assessment.IsActive;
                    addAssessment.ModifiedDateTime = DateTime.Now;
                    addAssessment.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                    db.SaveChanges();

                    return RedirectToAction(actionName: "ViewAssessment", controllerName: "Admin", routeValues: new { assessmentId = addAssessment.AssessmentID });
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


        public ActionResult SaveAssessment(Assessment assessment)
        {
            try
            {
                if (assessment != null && assessment.AssessmentID > 0)
                {
                    var editAssessment = db.Assessments.FirstOrDefault(a => a.AssessmentID == assessment.AssessmentID);

                    if (editAssessment != null)
                    {
                        editAssessment.Name = assessment.Name;
                        editAssessment.Category = assessment.Category;
                        editAssessment.Description = assessment.Description;
                        editAssessment.OutcomePassRate = assessment.OutcomePassRate;
                        editAssessment.CalculateOutcomePassRate = assessment.CalculateOutcomePassRate;
                        editAssessment.ProgramID = assessment.ProgramID;
                        editAssessment.IsActive = assessment.IsActive;
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

        public ActionResult AssessmentScheduling()
        {
            return View();
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

                    var FilteredUserSecurities = UserSecurities.Where(p => p.FirstName.ToLower().Contains(QueryString) || p.LastName.ToLower().Contains(QueryString) || p.IDNumber.Contains(QueryString));

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




        public ActionResult Assign([Required]int id)
        {
            var person = db.People.Where(p => id == p.PersonID).SingleOrDefault();
            ViewBag.PersonID = person.PersonID;
            ViewBag.Id = id;
            ViewBag.Name = person.FirstName + " " + person.LastName;

            return View(db.Roles);
        }

        public ActionResult RoleAssign(int person, List<short> roles)
        {
            var results = db.UserRoles.Where(ur => ur.PersonID == person);
            foreach (UserRole userRole in results)
            {
                db.UserRoles.Remove(userRole);
            }

            foreach (short role in roles)
            {
                UserRole user = new UserRole()
                {
                    PersonID = person,
                    RoleID = role
                };

                db.UserRoles.Add(user);
            }

            db.SaveChanges();

            return RedirectToAction(actionName: "AdminDashboard", controllerName: "Admin");
        }



        [HttpGet]
        public ActionResult ViewRoles()
        {
            var Roles = from Role in db.Roles
                        select Role;
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
                var Roles = from role in db.Roles
                            select role;
                var UserRoles = from userRole in db.UserRoles
                                where userRole.PersonID == UserID
                                select userRole;
                var User = (from user in db.Users
                            join person in db.People on user.PersonID equals person.PersonID
                            where user.PersonID == UserID
                            select new { user.PersonID, user.Login, person.FirstName, person.LastName, person.IdNumber }).FirstOrDefault();

                ManageUser Model = new ManageUser(User.PersonID, User.IdNumber, User.FirstName, User.LastName, Roles.ToList(), UserRoles.ToList());


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

            var People = from user in db.Users
                         join person in db.People on user.PersonID equals person.PersonID
                         select new { FirstName = person.FirstName, LastName = person.LastName, PersonID = person.PersonID, IDNumber = person.IdNumber };

            var UserRoles = from role in db.Roles
                            join userRole in db.UserRoles on role.RoleID equals userRole.RoleID
                            join user in db.Users on userRole.PersonID equals user.PersonID
                            select new { PersonID = user.PersonID, RoleName = role.Name, RoleID = role.RoleID };

            var PersonList = People.ToList();

            var UserRoleList = UserRoles.ToList();
            var UserSecurityList = new List<UserSecurity>();
            foreach (var person in PersonList)
            {
                var personRoles = new List<Role>();
                foreach (var userRole in UserRoleList)
                {
                    if (userRole.PersonID.Equals(person.PersonID))
                    {
                        Role role = new Role();
                        role.RoleID = userRole.RoleID;
                        role.Name = userRole.RoleName;
                        personRoles.Add(role);
                    }
                }
                UserSecurityList.Add(new UserSecurity(person.PersonID, person.IDNumber, person.FirstName, person.LastName, personRoles));
            }
            return UserSecurityList;

        }
    }
}