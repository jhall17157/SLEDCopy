using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RoleAdminController : SLEControllerBase
    {
        List<int> NonDeletableRoleIDs = new List<int>() {1,2};

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
        public ActionResult Delete(FormCollection form, short id)
        {
            // short id = short.Parse(form["id"]);
            var role = new Role();
            role = db.Roles.Where(r => r.RoleID == id).FirstOrDefault();
            var rolePermissions = db.RolePermissions.Where(rp => rp.RoleID == id);
            foreach (RolePermission rolePermission in rolePermissions)
            {
                db.RolePermissions.Remove(rolePermission);
            }
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

        [HttpPost]
        public ActionResult UpdateRole(FormCollection form, String submit, short roleID, short permissionID)
        {
            // Int16 RoleID = RoleID = Int16.Parse(form["roleID"]);
            // Int16 PermissionID = PermissionID = Int16.Parse(form["permissionID"]);
                switch (submit)
                {
                    case "add":
                        RolePermission rolePermission = new RolePermission
                        {
                            RoleID = roleID,
                            PermissionID = permissionID,
                            CreatedDateTime = DateTime.Now,
                            CreatedByLoginID = UserData.PersonId

                        };
                        db.RolePermissions.Add(rolePermission);

                        break;
                    case "delete":
                        var deletionEntry = (from RolePermission in db.RolePermissions
                                             where RolePermission.RoleID == roleID && RolePermission.PermissionID == permissionID
                                             select RolePermission).FirstOrDefault();
                        db.RolePermissions.Remove(deletionEntry);
                        break;
                }
			 db.SaveChanges();
			 return RedirectToAction("ManageRole", "RoleAdmin", new { id = roleID });
                // return Content("<html><script>window.location.href = '/RoleAdmin/ManageRole?id=" + RoleID.ToString() + "';</script></html>");
        }

        public ActionResult RoleAssign(int role, List<Permission> permissions)
        {
            var results = db.RolePermissions.Where(rp => rp.RoleID == role);
            foreach (RolePermission rolePermission in results)
            {
                db.RolePermissions.Remove(rolePermission);
            }

            foreach (Permission permission in permissions)
            {
                RolePermission rolePermission = new RolePermission()
                {
                    RoleID = (short)role,
                    PermissionID = permission.PermissionID
                };

                db.RolePermissions.Add(rolePermission);
            }

            db.SaveChanges();

            return RedirectToAction(actionName: "AdminDashboard", controllerName: "Admin");
        }

        public ActionResult EditRoles(int id, EditRoleViewModel viewModel)
        {

            viewModel = new EditRoleViewModel();
            viewModel.role = db.Roles.Where(r => r.RoleID == id).FirstOrDefault();
            if (viewModel.role != null)
            {
                viewModel.schoolSecurities = db.SchoolSecurities.Where(sc => sc.RoleID == id).ToList();
                viewModel.departmentSecurities = db.DepartmentSecurities.Where(ds => ds.RoleID == id).ToList();
                viewModel.programSecurities = db.ProgramSecurities.Where(ps => ps.RoleID == id).ToList();
                viewModel.allDepartments = db.Departments.OrderBy(d => d.Name).ToList();
                viewModel.allPrograms = db.Programs.OrderBy(p => p.Name).ToList();
                viewModel.allSchools = db.Schools.OrderBy(s => s.Name).ToList();
            }
            else
            {
                return RedirectToAction("ManageRoles");
            }
            return View(viewModel);
        }

        public JsonResult updateRoleNameAndDescription(int roleID, string updatedRoleName, string updatedRoleDescription)
        {
            Role targetRole = db.Roles.Where(r => r.RoleID == roleID).FirstOrDefault();
            if(targetRole != null)
            {
                targetRole.Name = updatedRoleName;
                targetRole.Description = updatedRoleDescription;
                db.SaveChanges();
                return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return null;
            }
        }

        public JsonResult toggleSchoolSecurity(int roleID, int SchoolID)
        {
            Role targetRole = db.Roles.Where(r => r.RoleID == roleID).FirstOrDefault();
            if (targetRole != null)
            {
                SchoolSecurity targetSchoolSec = targetRole.SchoolSecurities.Where(ss => ss.SchoolID == SchoolID).FirstOrDefault();
                if(targetSchoolSec != null)
                {
                    db.SchoolSecurities.Remove(targetSchoolSec);
                    db.SaveChanges();
                    return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    SchoolSecurity newSchoolSec = new SchoolSecurity();
                    newSchoolSec.RoleID = (short)roleID;
                    newSchoolSec.SchoolID = (byte)SchoolID;
                    newSchoolSec.CreatedDateTime = DateTime.Now;
                    newSchoolSec.ModifiedDateTime = DateTime.Now;
                    newSchoolSec.CreatedByLoginID = UserData.PersonId;
                    newSchoolSec.ModifiedByLoginID = UserData.PersonId;
                    db.SchoolSecurities.Add(newSchoolSec);
                    db.SaveChanges();
                    return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                return null;
            }
        }

        public JsonResult toggleProgramSecurity(int roleID, int ProgramID)
        {
            Role targetRole = db.Roles.Where(r => r.RoleID == roleID).FirstOrDefault();
            if (targetRole != null)
            {
                ProgramSecurity targetProgramSec = targetRole.ProgramSecurities.Where(ps => ps.ProgramID == ProgramID).FirstOrDefault();
                if (targetProgramSec != null)
                {
                    db.ProgramSecurities.Remove(targetProgramSec);
                    db.SaveChanges();
                    return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    ProgramSecurity newProgramSec = new ProgramSecurity();
                    newProgramSec.RoleID = (short)roleID;
                    newProgramSec.ProgramID = (byte)ProgramID;
                    newProgramSec.CreatedDateTime = DateTime.Now;
                    newProgramSec.ModifiedDateTime = DateTime.Now;
                    newProgramSec.CreatedByLoginID = UserData.PersonId;
                    newProgramSec.ModifiedByLoginID = UserData.PersonId;
                    db.ProgramSecurities.Add(newProgramSec);
                    db.SaveChanges();
                    return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                return null;
            }
        }

        public JsonResult toggleDepartmentSecurity(int roleID, int DepartmentID)
        {
            Role targetRole = db.Roles.Where(r => r.RoleID == roleID).FirstOrDefault();
            if (targetRole != null)
            {
                DepartmentSecurity targetDepartmentSec = targetRole.DepartmentSecurities.Where(ds => ds.DepartmentID == DepartmentID).FirstOrDefault();
                if (targetDepartmentSec != null)
                {
                    db.DepartmentSecurities.Remove(targetDepartmentSec);
                    db.SaveChanges();
                    return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    DepartmentSecurity newDepartmentSec = new DepartmentSecurity();
                    newDepartmentSec.RoleID = (short)roleID;
                    newDepartmentSec.DepartmentID = (byte)DepartmentID;
                    newDepartmentSec.CreatedDateTime = DateTime.Now;
                    newDepartmentSec.ModifiedDateTime = DateTime.Now;
                    newDepartmentSec.CreatedByLoginID = UserData.PersonId;
                    newDepartmentSec.ModifiedByLoginID = UserData.PersonId;
                    db.DepartmentSecurities.Add(newDepartmentSec);
                    db.SaveChanges();
                    return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
            else
            {
                return null;
            }
        }


        public ActionResult CreateRole()
        {
            return View();
        }

        public ActionResult ManageRoles(ManageRolesViewModel vm)
        {
            if(vm!= null && vm.SearchTerm != null && vm.SearchTerm != "")
            {
                vm.NonDeletableRoles = db.Roles.Include("UserRoles").Where(r => r.Name.ToLower().Contains(vm.SearchTerm.ToLower())&& NonDeletableRoleIDs.Contains(r.RoleID)).OrderBy(r => r.Name.ToLower()).ToList();
                vm.DeletableRoles = db.Roles.Include("UserRoles").Where(r => r.Name.ToLower().Contains(vm.SearchTerm.ToLower()) && !NonDeletableRoleIDs.Contains(r.RoleID)).OrderBy(r => r.Name.ToLower()).ToList();
            }
            else
            {
                vm = new ManageRolesViewModel();
                vm.NonDeletableRoles = db.Roles.Include("UserRoles").Where(r => NonDeletableRoleIDs.Contains(r.RoleID)).ToList();
                vm.DeletableRoles =  db.Roles.Include("UserRoles").Where(r => !NonDeletableRoleIDs.Contains(r.RoleID)).ToList();
            }
            //vm.SearchTerm = "";
            return View(vm);
        }

        public JsonResult DeleteRole(int TargetID)
        {
            try
            {
                Role targetRole = db.Roles.Where(r => r.RoleID == TargetID).FirstOrDefault();
                if(targetRole != null)
                {
                    db.Roles.Remove(targetRole);
                    db.SaveChanges();
                    return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                return null;
            } catch (Exception e)
            {
                return null;
            }
        }

        public ActionResult ManageRoleMembership()
        {
            return View();
        }
    }
}