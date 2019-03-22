using CLS_SLE.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
            return View();
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
        public ActionResult ManageUserRoles()
        {
            try
            {
                dynamic MyModel = new ExpandoObject();
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

                        MyModel.UserSecurityList = FilteredUserSecurities;
                    }
                    catch
                    {
                        throw new Exception("Error fetching user list");
                    }
                }

                else
                {
                    MyModel.UserSecurityList = GetUserSecurities();
                }
                return View(MyModel);
            }
            catch
            {
                logger.Error("Error fetching user List");
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