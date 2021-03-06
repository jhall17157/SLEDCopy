using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Users")]
    public class AdminUserController : SLEControllerBase
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        private readonly int PageSize = 10;

        //public ActionResult Index() => View(db.Users.OrderBy(u => u.Login));
        [Authorize(Roles = "CreateManageUsers")]
        public ActionResult Create() => View();

        [Authorize(Roles = "CreateManageUsers")]
        public ActionResult Edit(short id)
        {
            int UserID = id;
            var UserRoles = db.UserRoles.Where(ur => ur.PersonID == id).ToList();
            var Roles = db.UserRoles.Where(ur => ur.PersonID == id).Select(r => r.Role.Name).ToList();
            var User = db.Users.Where(u => u.Person.PersonID == id).FirstOrDefault();
            var Person = db.People.Where(p => p.PersonID == id).FirstOrDefault();

            UpdateUserViewModel model = new UpdateUserViewModel();
            model.Person = Person;
            model.Roles = Roles;
            model.UserRoles = UserRoles;
            model.User = User;

            return View(model);
        }

        /**
         * Lucas Nolting
         * This is a method I will be converting to use Model binding
         */
        [Authorize(Roles = "CreateManageUsers")]
        [HttpPost]
        public ActionResult CreateUser(AddUserViewModel userVM)
        {

            if (ModelState.IsValid)
            {
                //person
                userVM.Person.CreatedDateTime = DateTime.Now;
                userVM.Person.CreatedByLoginID = UserData.PersonId;
                Person existingPerson = db.People.FirstOrDefault(u => u.IdNumber == userVM.Person.IdNumber);
                //Checks if Existing Person, and Creates if Not, Updates if is
                if (existingPerson==null)
                {
                    db.People.Add(userVM.Person);
                    db.SaveChanges();
                    userVM.User.PersonID = userVM.Person.PersonID;

                }
                else
                {
                    existingPerson.FirstName = userVM.Person.FirstName;
                    existingPerson.LastName = userVM.Person.LastName;
                    existingPerson.ModifiedDateTime = DateTime.Now;
                    existingPerson.ModifiedByLoginID = UserData.PersonId;
                    db.SaveChanges();
                    userVM.User.PersonID = existingPerson.PersonID;
                }
            
                //user
                userVM.User.CreatedDateTime = DateTime.Now;
                userVM.User.CreatedByLoginID = UserData.PersonId;
                //hash pass on submission
               // userVM.HashStudentID(userVM.Person.IdNumber);
                db.Users.Add(userVM.User);
                db.SaveChanges();

            }

            else
            {
                return RedirectToAction("Create", "AdminUser");
            }

            return RedirectToAction("ManageUsers", "AdminUser");
        }

        /**
         * TODO Document this and all other model bound method
         */
        [Authorize(Roles = "ManageUserRoles")]
        [HttpPost]
        public JsonResult RemoveUserRole(string roleName, short id)
        {
            var roleID = db.Roles.Where(r => r.Name == roleName).Select(r => r.RoleID).FirstOrDefault();
            var deletionEntry = db.UserRoles.Where(ur => ur.PersonID == id && ur.RoleID == roleID).FirstOrDefault();
            db.UserRoles.Remove(deletionEntry);

            db.SaveChanges();

            return new JsonResult { Data = true };
        }
        public ActionResult UpdateUser(UpdateUserViewModel updateUserViewModel, short id)
        {
            User editUser = db.Users.Where(u => u.PersonID == id).FirstOrDefault();
            Person editPerson = db.People.Where(u => u.PersonID == id).FirstOrDefault();

            editPerson.FirstName = updateUserViewModel.Person.FirstName;
            editPerson.LastName = updateUserViewModel.Person.LastName;
            editPerson.IdNumber = updateUserViewModel.Person.IdNumber;
            editPerson.ModifiedByLoginID = UserData.PersonId;
            editPerson.ModifiedDateTime = DateTime.Now;
            editUser.Login = updateUserViewModel.User.Login;
            editUser.Email = updateUserViewModel.User.Email;
            editUser.IsActive = updateUserViewModel.User.IsActive;
            editUser.ModifiedDateTime = DateTime.Now;
            editUser.ModifiedByLoginID = UserData.PersonId;

            db.SaveChanges();

            return RedirectToAction("ManageUsers", "AdminUser");
        }

        [Authorize(Roles = "CreateManageUsers")]
        [HttpPost]


        public ActionResult Activate(ViewUserViewModel viewUserViewModel, short id)
        {

            //int id = Int32.Parse(form["id"]);
            viewUserViewModel.User = db.Users.Where(u => u.PersonID == id).SingleOrDefault();

            if (viewUserViewModel.User.IsActive)
            {
                viewUserViewModel.User.IsActive = false;
                db.SaveChanges();
            }
            else
            {
                viewUserViewModel.User.IsActive = true;
                db.SaveChanges();
            }


            return RedirectToAction("ManageUsers", "AdminUser");
        }

        public JsonResult SetUserActiveStatus(int PersonID, bool IsActive)
        {

            User targetUser = db.Users.Where(u => u.PersonID == PersonID).FirstOrDefault();
            try
            {
                if (targetUser != null)
                {
                    targetUser.IsActive = IsActive;
                    db.SaveChanges();
                    return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /*
        public ActionResult ManageUsers()
        {
            ManageUsersViewModel vm = new ManageUsersViewModel();
            vm.Users = db.Users.Include("Person").OrderBy(u => u.Login).ToList();
            return View(vm);
        }
        */
        public ActionResult ManageUsers(ManageUsersViewModel vm, int page)
        {
            
            int ResultsCount =1;
            if (vm.SearchTerm != null)
            {
                vm.Users = db.Users.Include("Person").Where(u => u.Login.ToLower().Contains(vm.SearchTerm.ToLower())||u.PersonID.ToString().ToLower().StartsWith(vm.SearchTerm.ToLower())||(u.Person.LastName + ", " + u.Person.FirstName).ToLower().Contains(vm.SearchTerm.ToLower())).OrderBy(u => u.Person.LastName).ToList();
                ResultsCount = db.Users.Include("Person").OrderBy(u => u.Person.LastName).Count();
            }
            else
            {
                vm.Users = db.Users.Include("Person").OrderBy(u => u.Person.LastName).Skip((page - 1) * PageSize).Take(PageSize).ToList();
                ResultsCount = db.Users.Include("Person").OrderBy(u => u.Person.LastName).Count();
            }
            vm.PagingInfo = new ViewModels.PagingInfo { CurrentPage = page, ItemsPerPage = PageSize, TotalItems = ResultsCount };

            //vm.SearchTerm = "";
            return View(vm);
        }
    }
}