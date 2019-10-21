﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.Models;
using BCrypt;
using CLS_SLE.ViewModels;

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

        /**
         * Lucas Nolting
         * This is a method I will be converting to use Model binding
         */
        [HttpPost]
        public ActionResult CreateUser(AddUserViewModel userVM)
        {

            if(ModelState.IsValid)
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
    }
}