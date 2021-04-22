using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Dynamic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Roles")]
    public class RoleAdminController : SLEControllerBase
    {
        List<int> NonDeletableRoleIDs = new List<int>() {1,2};

        SLE_TrackingEntities db = new SLE_TrackingEntities();
        // public ActionResult Index() => View(db.Roles);

        [Authorize(Roles = "AddRoles")]
        public ActionResult Create() => View();

        public ActionResult Confirm([Required]int id)
        {
            var role = db.Roles.Where(r => id == r.RoleID).SingleOrDefault();
            ViewBag.Id = id;
            ViewBag.Name = role.Name;

            return View();
        }

        [Authorize(Roles = "AddRoles")]
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
            return RedirectToAction(actionName: "ManageRoles", controllerName: "RoleAdmin");
        }

        [Authorize(Roles = "EditRoles")]
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
            return RedirectToAction(actionName: "ManageRoles", controllerName: "RoleAdmin");
        }

        [Authorize(Roles = "ManageUserRoles")]
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

        [Authorize(Roles = "EditRoles")]
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
        [Authorize(Roles = "EditRoles")]
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
        [Authorize(Roles = "EditRoles")]
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

        [Authorize(Roles = "EditRoles")]
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

        [Authorize(Roles = "EditRoles")]
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

        [Authorize(Roles = "AddRoles")]
        public ActionResult CreateRoles( CreateRoleViewModel vm)
        {

            return View(vm);
        }

        [Authorize(Roles = "AddRoles")]
        public ActionResult SubmitRoleCreation(CreateRoleViewModel vm)
        {
            try
            {
                if(vm.role != null && vm.role.Name != null && vm.role.Name != "" )
                {
                    db.Roles.Add(vm.role);
                    db.SaveChanges();
                    return RedirectToAction("ManageRoles", "RoleAdmin");
                }
                else
                {
                    return View("CreateRole", vm);
                }
            } catch (Exception e)
            {
                return View("CreateRole",vm);
            }
        }

        [Authorize(Roles = "EditRoles")]
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

        public JsonResult getRelatedSchoolSecurities(int roleID)
        {
            Role targetRole = db.Roles.Where(r => r.RoleID == roleID).FirstOrDefault();
            if (targetRole != null)
            {
                List<SchoolSecurity> schoolSecurities = targetRole.SchoolSecurities.OrderBy(ss => ss.School.Name).ToList();
                if(schoolSecurities.Count() > 0)
                {

                    List<SchoolTruncated> allSchoolsOutput = new List<SchoolTruncated>();
                    foreach (SchoolSecurity s in schoolSecurities)
                    {
                        allSchoolsOutput.Add(new SchoolTruncated() { SchoolID = s.SchoolID, Name = s.School.Name });
                    }
                    return new JsonResult { Data = allSchoolsOutput, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return null;
                }
                
            }
            else
            {
                return null;
            }
        }

        public JsonResult getRelatedDepartmentSecurities(int roleID)
        {
            Role targetRole = db.Roles.Where(r => r.RoleID == roleID).FirstOrDefault();
            if (targetRole != null)
            {
                List<DepartmentSecurity> departmentSecurities = targetRole.DepartmentSecurities.OrderBy(ds => ds.Department.Name).ToList();
                if (departmentSecurities.Count() > 0)
                {
                    List<DepartmentTruncated> allDepartmentsOutput = new List<DepartmentTruncated>();
                    foreach (DepartmentSecurity d in departmentSecurities)
                    {
                        allDepartmentsOutput.Add(new DepartmentTruncated() { DepartmentID = d.DepartmentID, Name = d.Department.Name });
                    }
                    return new JsonResult { Data = allDepartmentsOutput, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
            else
            {
                return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult getRelatedProgramSecurities(int roleID)
        {
            Role targetRole = db.Roles.Where(r => r.RoleID == roleID).FirstOrDefault();
            if (targetRole != null)
            {
                List<ProgramSecurity> programSecurities = targetRole.ProgramSecurities.OrderBy(ps => ps.Program.Name).ToList();
                if (programSecurities.Count() > 0)
                {
                    List<ProgramTruncated> allProgramsOutput = new List<ProgramTruncated>();
                    foreach (ProgramSecurity ps in programSecurities)
                    {
                        allProgramsOutput.Add(new ProgramTruncated() { ProgramID = ps.ProgramID, Name = ps.Program.Name });
                    }
                    return new JsonResult { Data = allProgramsOutput, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

            }
            else
            {
                return new JsonResult { Data = null, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        [Authorize(Roles = "EditRoles")]
        public JsonResult DeleteRole(int TargetID)
        {
            try
            {
                Role targetRole = db.Roles.Where(r => r.RoleID == TargetID).FirstOrDefault();
                if(targetRole != null)
                {
                    //targetRole.UserRoles = null;
                    IQueryable<UserRole> affectedUserRoles = db.UserRoles.Where(ur => ur.RoleID == targetRole.RoleID);
                    db.UserRoles.RemoveRange(affectedUserRoles);
                    db.SaveChanges();
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

        [Authorize(Roles = "ManageUserRoles")]
        public ActionResult ManageRoleMembership(int? roleID, string searchTerm)
        {
            var ManageRoleMembershipViewModel = new ManageRoleMembershipViewModel();
            if (roleID.HasValue)
            {
                ManageRoleMembershipViewModel.RoleID = roleID;
            }

            if (!(ManageRoleMembershipViewModel.RoleID.HasValue))
            {
                ManageRoleMembershipViewModel.RoleID = 1;
            }

            ManageRoleMembershipViewModel.SearchTerm = searchTerm;
            if (ManageRoleMembershipViewModel != null && ManageRoleMembershipViewModel.SearchTerm != null && ManageRoleMembershipViewModel.SearchTerm != "")
            {
                var UsersInRole = GetUserSecurities()
                    .Where(p => p.Roles.Any(r => r.RoleID == ManageRoleMembershipViewModel.RoleID));

                var FirstNameList = UsersInRole.Where(u => u.FirstName.ToLower().Contains(ManageRoleMembershipViewModel.SearchTerm.ToLower()));
                var lastNameList = UsersInRole.Where(u => u.LastName.ToLower().Contains(ManageRoleMembershipViewModel.SearchTerm.ToLower()));
                var LoginList = UsersInRole.Where(u => u.Login.ToLower().Contains(ManageRoleMembershipViewModel.SearchTerm.ToLower()));
                var IDList = UsersInRole.Where(u => u.IDNumber.ToLower().Contains(ManageRoleMembershipViewModel.SearchTerm.ToLower()));

                ManageRoleMembershipViewModel.UsersInRole = FirstNameList.Union(lastNameList).Union(LoginList).Union(IDList).OrderBy(u => u.LastName).ToList();
            }
            else
            {
                ManageRoleMembershipViewModel.UsersInRole = GetUserSecurities().Where(p => p.Roles.Any(r => r.RoleID == ManageRoleMembershipViewModel.RoleID)).OrderBy(u => u.LastName).ToList();
            }

            var CurrentRole = (from Role in db.Roles
                               where Role.RoleID == ManageRoleMembershipViewModel.RoleID
                               select Role).FirstOrDefault();
            ManageRoleMembershipViewModel.CurrentRole = CurrentRole;

            return View(ManageRoleMembershipViewModel);
        }

        public JsonResult UserAutoComplete(string search, int roleID)
        {


            var UsersNotInRole = GetUserSecurities().Where(u => u.Roles.All(r => r.RoleID != roleID));

            List<UserRoleSearchModel> resultUsers = UsersNotInRole.Where(p => (p.Login.Contains(search) || 
                p.LastName.Contains(search) || 
                p.FirstName.Contains(search) || 
                p.IDNumber.Contains(search)))
                .Select(p => new UserRoleSearchModel
                {
                login = p.Login,
                personID = p.PersonID,
                idNumber = p.IDNumber,
                lastName = p.LastName,
                firstName = p.FirstName
            }).ToList();

            return new JsonResult { Data = resultUsers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult UserListInfo(string search, int roleID)
        {
            var DataUser = new RoleMembershipUserModel();
            if (String.IsNullOrEmpty(search))
            {
                DataUser.message = "Please Enter A Valid ID";
                DataUser.success = false;
            }
            else
            {
                int personID = 0;
                if (Int32.TryParse(search, out personID))
                {
                    var User = GetUserSecurities().ToList().Where(p => p.IDNumber == search).FirstOrDefault();

                    if (User == null) { DataUser.message = "No User has " + search + " as an ID"; DataUser.success = false; }
                    else
                    {
                        if (!(User.Roles.Any(r => r.RoleID == roleID)))
                        {
                            DataUser = new RoleMembershipUserModel { login = User.Login, firstName = User.FirstName, lastName = User.LastName, id = User.IDNumber, PID = User.PersonID };
                            DataUser.success = true;
                        }
                        else
                        {
                            DataUser.message = "User with ID: " + User.IDNumber + " is already a member of this role.";
                            DataUser.success = false;
                        }
                    }
                }
            }
            return new JsonResult { Data = DataUser, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize (Roles = "ManageUserRoles")]
        public ActionResult AddUserToRole( string personID, string roleID)
        {
            short pID = 0;
            short rID = 0;

            Int16.TryParse(personID, out pID);
            Int16.TryParse(roleID, out rID);

            try
            {
                UserRole userRole = new UserRole
                {
                    PersonID = pID,
                    RoleID = rID,
                    CreatedDateTime = DateTime.Now,
                    CreatedByLoginID = UserData.PersonId
                };

                db.UserRoles.Add(userRole);

                db.SaveChanges();
                return RedirectToAction("ManageRoleMembership", "RoleAdmin", new { roleID = rID });
            }
            catch(Exception e)
            {
                return RedirectToAction("ManageRoleMembership", "RoleAdmin", new { roleID = rID });
            }
        }

        [Authorize(Roles = "ManageUserRoles")]
        public ActionResult RemoveUserFromRole(string personID, string roleID)
        {
            short pID = 0;
            short rID = 0;

            Int16.TryParse(personID, out pID);
            Int16.TryParse(roleID, out rID);

            try
            {

                if(rID == 2 && GetUserSecurities().Where(p => p.Roles.Any(r => r.RoleID == rID)).ToList().Count() == 1)
                {
                    //dont want to delete final member in administrator
                }
                else
                {
                    var deletionEntry = (from UserRole in db.UserRoles
                                         where UserRole.PersonID == pID && UserRole.RoleID == rID
                                         select UserRole).FirstOrDefault();
                    db.UserRoles.Remove(deletionEntry);
                    db.SaveChanges();
                }
                return RedirectToAction("ManageRoleMembership", new { roleID = rID });
            }
            catch (Exception e)
            {
                return RedirectToAction("ManageRoleMembership", new { roleID = rID });
            }
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