using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminUserController : SLEControllerBase
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();


        public ActionResult Index() => View(db.Users.OrderBy(u => u.Login));

        public ActionResult Create() => View();

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
        [HttpPost]
        public ActionResult CreateUser(AddUserViewModel userVM)
        {

            if (ModelState.IsValid)
            {
                //person
                userVM.Person.CreatedDateTime = DateTime.Now;
                db.People.Add(userVM.Person);

                //user
                userVM.User.CreatedDateTime = DateTime.Now;
                //hash pass on submission
                userVM.HashStudentID(userVM.Person.IdNumber);
                db.Users.Add(userVM.User);
                db.SaveChanges();

            }

            else
            {
                return RedirectToAction("Create", "AdminUser");
            }

            return RedirectToAction("ViewUsers", "Admin");
        }

        /**
         * TODO Document this and all other model bound method
         */
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
            editUser.Login = updateUserViewModel.User.Login;
            editUser.Email = updateUserViewModel.User.Email;
            editUser.IsActive = updateUserViewModel.User.IsActive;

            db.SaveChanges();

            return RedirectToAction("ViewUsers", "Admin");
        }

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


            return RedirectToAction("ViewUsers", "Admin");
        }
    }
}