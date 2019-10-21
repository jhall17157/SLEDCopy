using CLS_SLE.Models;
using NLog;
using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CLS_SLE.ViewModels;

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

        [HttpGet]
        public ActionResult ViewUsers(ViewUserViewModel viewUserViewModel)
        {

            //dynamic Model = new ExpandoObject();
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

                    viewUserViewModel.UserSecurities = (List<UserSecurity>) FilteredUserSecurities;
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
