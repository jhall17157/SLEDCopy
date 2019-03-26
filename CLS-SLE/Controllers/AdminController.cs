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

namespace CLS_SLE.Controllers
{
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
            return View();
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

        public ActionResult UsersSecurity()
        {
            try
            {
                var People = from user in db.Users
                             join person in db.People on user.PersonID equals person.PersonID
                             select new { FirstName = person.FirstName, LastName = person.LastName, PersonID = person.PersonID, IDNumber = person.IdNumber };
                var UserRoles = from role in db.Roles
                                join userRole in db.UserRoles on role.RoleID equals userRole.RoleID
                                join user in db.Users on userRole.PersonID equals user.PersonID
                                select new { PersonID = user.PersonID, RoleName = role.Name, RoleID = role.RoleID };

                dynamic MyModel = new ExpandoObject();
                var PersonList = People.ToList();
                var UserRoleList = UserRoles.ToList();
                var UserSecurityList = new List<UserSecurity>();
                foreach(var person in PersonList)
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

                MyModel.UserSecurityList = UserSecurityList;

                return View(MyModel);
            }
            catch
            {
                logger.Error("Error fetching user List");
                return Exceptions();
            }
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
            foreach(UserRole userRole in results)
            {
                db.UserRoles.Remove(userRole);
            }

            foreach(short role in roles)
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

        private ActionResult Exceptions()
        {
            return RedirectToAction(actionName: "AdminDashboard", controllerName: "Admin");
        }


    }
}