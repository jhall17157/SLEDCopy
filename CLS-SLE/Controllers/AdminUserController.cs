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

        public ActionResult Edit(short id) {
            User user = db.Users.Where(u => u.PersonID == id).FirstOrDefault();
            Person person = db.People.Where(p => p.PersonID == id).FirstOrDefault();
            ViewBag.Id = user.PersonID;
            ViewBag.First = person.FirstName;
            ViewBag.Last = person.LastName;
            ViewBag.Email = user.Email;
            ViewBag.Login = user.Login;
            return View();
        }

        public ActionResult CreateUser(FormCollection form)
        {
            string fName = form["fName"];
            string lName = form["lName"];
            string login = form["login"];
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

        public ActionResult UpdateUser(FormCollection form)
        {
            short id = Int16.Parse(form["id"]);
            string fName = form["fName"];
            string lName = form["lName"];
            string email = form["email"];

            User updateU = db.Users.Where(u => u.PersonID == id).FirstOrDefault();
            Person updateP = db.People.Where(p => p.PersonID == id).FirstOrDefault();

            updateU.Email = email;
            updateP.FirstName = fName;
            updateP.LastName = lName;

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

            return RedirectToAction("ViewUsers", "Admin");
        }
    }
}