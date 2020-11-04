using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using System;
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
            User user = db.Users.Where(u => u.PersonID == id).FirstOrDefault();
            Person person = db.People.Where(p => p.PersonID == id).FirstOrDefault();
            ViewBag.Id = user.PersonID;
            ViewBag.First = person.FirstName;
            ViewBag.Last = person.LastName;
            ViewBag.Email = user.Email;
            ViewBag.Login = user.Login;
            return View();
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
                    db.SaveChanges();
                    userVM.User.PersonID = existingPerson.PersonID;
                }
                //db.People.Add(userVM.Person);

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
        public ActionResult UpdateUser(UpdateUserViewModel updateUserViewModel, short id)
        {
            User editUser = db.Users.Where(u => u.PersonID == id).FirstOrDefault();
            Person editPerson = db.People.Where(u => u.PersonID == id).FirstOrDefault();

            editPerson.FirstName = updateUserViewModel.Person.FirstName;
            editPerson.LastName = updateUserViewModel.Person.LastName;
            editUser.Login = updateUserViewModel.User.Login;
            editUser.Email = updateUserViewModel.User.Email;

            db.SaveChanges();

            return RedirectToAction("ViewUsers", "Admin");
        }

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
        public ActionResult ManageUsers(ManageUsersViewModel vm)
        {
            if(vm.SearchTerm != null)
            {
                vm.Users = db.Users.Include("Person").Where(u => u.Login.ToLower().Contains(vm.SearchTerm.ToLower())||u.PersonID.ToString().ToLower().StartsWith(vm.SearchTerm.ToLower())||(u.Person.LastName + ", " + u.Person.FirstName).ToLower().Contains(vm.SearchTerm.ToLower())).OrderBy(u => u.Login).ToList();
            }
            else
            {
                vm.Users = db.Users.Include("Person").OrderBy(u => u.Login).ToList();
            }
            //vm.SearchTerm = "";
            return View(vm);
        }


        public ActionResult CreateUser()
        {
            return View();
        }

        public ActionResult EditUser()
        {
            return View();
        }
    }
}