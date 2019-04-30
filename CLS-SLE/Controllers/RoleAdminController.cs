using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using CLS_SLE.Models;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RoleAdminController : Controller
    {
        SLE_TrackingEntities db = new SLE_TrackingEntities();
        public ActionResult Index() => View(db.Roles);


        public ActionResult Create() => View();

        public ActionResult Confirm([Required]int id)
        {
            var role = db.Roles.Where(r => id == r.RoleID).SingleOrDefault();
            ViewBag.Id = id;
            ViewBag.Name = role.Name;

            return View();
        }
        

        [HttpPost]
        public async Task<ActionResult> Create([Required]string name, [Required]string desc)
        {
            if (ModelState.IsValid)
            {
                    var role = new Role()
                    {
                        Name = name,
                        Description = desc
                    };

                    db.Roles.Add(role);
                    db.SaveChanges();
            }
            return View("Index", db.Roles);
        }

        [HttpPost]
        public ActionResult Delete(FormCollection form)
        {
            short id = short.Parse(form["id"]);
            var role = new Role();
            role = db.Roles.Where(r => r.RoleID == id).FirstOrDefault();
            db.Roles.Remove(role);
            db.SaveChanges();
            return RedirectToAction(actionName: "Index", controllerName: "RoleAdmin");
        }

        [HttpGet]
        public ActionResult ManageRole(int id)
        {

                int RoleID = id;
                var Permissions = (from permission in db.Permissions
                             select permission).OrderBy(r => r.Name);
                var RolePermissions = from rolePermissions in db.RolePermissions
                                where rolePermissions.RoleID == RoleID
                                select rolePermissions;
                var Role = (from role in db.Roles where role.RoleID == RoleID select role).FirstOrDefault();

                ManageRole Model = new ManageRole(Role.RoleID, Role.Name, Permissions.ToList(), RolePermissions.ToList());


                return View(Model);
        }

        public ActionResult RoleAssign(int role, List<short> permissions)
        {
            var results = db.RolePermissions.Where(rp => rp.RoleID == role);
            foreach (RolePermission rolePermission in results)
            {
                db.RolePermissions.Remove(rolePermission);
            }

            foreach (short permission in permissions)
            {
                RolePermission rolePermission = new RolePermission()
                {
                    RoleID = (short)role,
                    PermissionID = permission
                };

                db.RolePermissions.Add(rolePermission);
            }

            db.SaveChanges();

            return RedirectToAction(actionName: "AdminDashboard", controllerName: "Admin");
        }
    }
}