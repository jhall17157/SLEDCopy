﻿using CLS_SLE.Models;
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
            return View();
        }

        public ActionResult AssessmentsScheduling()
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