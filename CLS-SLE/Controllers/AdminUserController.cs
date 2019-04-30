using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.Models;
using BCrypt;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminUserController : Controller
    {
        SLE_TrackingEntities db = new SLE_TrackingEntities();
        

        public ActionResult Index() => View(db.Users.OrderBy(u => u.Login));

        public ActionResult Create() => View();

        public ActionResult Edit() => View();

        public ActionResult CreateUser(FormCollection form)
        {
            string fName = form["fName"];
            string lName = form["lName"];
            string login = fName.Substring(0, 1) + lName;
            login = login.ToLower();
            string id = form["id"];
            string email = form["email"];
            DateTime created = DateTime.Now;
            string hash = BCrypt.Net.BCrypt.HashString(id);

            Person person = new Person()
            {
                FirstName = fName,
                LastName = lName,
                IdNumber = id,
                CreatedDateTime = created
            };
            db.People.Add(person);
            User user = new User()
            {
                PersonID = person.PersonID,
                Login = login,
                Email = email,
                MustResetPassword = true,
                CreatedDateTime = created,
                Hash = hash
            };
            db.Users.Add(user);
            db.SaveChanges();

            return RedirectToAction("Index", "AdminUser");
        }

        public ActionResult Activate(FormCollection form)
        {
            int id = Int32.Parse(form["id"]);

            User user = db.Users.Where(u => u.PersonID == id).SingleOrDefault();

            if (user.IsActive)
            {
                user.IsActive = false;
                db.SaveChanges();
            }
            else
            {
                user.IsActive = true;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "AdminUser");
        }
    }
}