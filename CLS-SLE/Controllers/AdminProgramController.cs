using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminProgramController : SLEControllerBase
    {
        private readonly int PageSize = 20;

        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();

        // GET: AdminProgram
        /// <summary>
        ///       http get request that sends the AdminProgram/Programs view which displays a list of the programs
        /// </summary>
        /// <returns>
        ///       a view of programs that contains a list of programs ordered by the program's name
        /// </returns>
        public ActionResult Programs(int page, string search, string updatedMessage, string addedName)
        {
            ProgramListViewModel programsViewModel = new ProgramListViewModel();

            int ResultsCount;
                if (search == null)
                {
                    //creating a new CoursesViewModel - the Courses is a list of Courses sorted by their Number and does not include any courses with a "000-000" Number
                    //the "000-000" Numbered Courses were imported to tie some old assessment data to that was needed in the system
                    programsViewModel.Programs = db.Programs.OrderBy(p => p.Number).Skip((page - 1) * PageSize).Take(PageSize);
                    ResultsCount = db.Programs.Count();
                }
                else
                {
                    programsViewModel.Programs = db.Programs.Where(p => (p.Name.Contains(search) || p.Number.Contains(search))).OrderBy(c => c.Number).Skip((page - 1) * PageSize).Take(PageSize);
                    ResultsCount = db.Programs.Where(p => (p.Name.Contains(search) || p.Number.Contains(search))).Count();
                }

            //the Paging info is going to contain all the information required for pagination
            programsViewModel.PagingInfo = new ViewModels.PagingInfo { CurrentPage = page, ItemsPerPage = PageSize, TotalItems = ResultsCount };

            programsViewModel.SearchInput = search;

            if (updatedMessage != null)
            {
                programsViewModel.updatedMessage = updatedMessage;
                if (programsViewModel.updatedMessage == "success") { programsViewModel.alertMessage = (addedName + " was added!"); }
                else { programsViewModel.alertMessage = (addedName+" already exsists!"); }
            }

            return View(programsViewModel);
        }

        [HttpPost]
        public ActionResult SearchProgram(ProgramSearchViewModel searchVM)
        {

            if (db.Programs.Where(p => !p.Name.Contains("Folio180")).Where(p => p.Name.Contains(searchVM.SearchInput)) != null ||
                db.Programs.Where(p => !p.Name.Contains("Folio180")).Where(p => p.Number.Contains(searchVM.SearchInput)) != null)
            {
                return RedirectToAction("Programs", "AdminProgram", new { page = 1, search = searchVM.SearchInput, department = searchVM.DepartmentFilter });
            }
            else { return RedirectToAction("ProgramSearchError", "AdminProgram", new { search = searchVM.SearchInput }); }
        }

        public JsonResult ProgramAutoComplete(string search)
        {
            List<ProgramSearchModel> resultCourses = db.Programs.Where(p => !p.Name.Contains("Folio180")).Where(p => (p.Name.Contains(search) || p.Number.Contains(search))).Select(p => new ProgramSearchModel
            {
                id = p.ProgramID,
                name = p.Name
            }).ToList();

            return new JsonResult { Data = resultCourses, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: AdminProgram/AddProgram
        /// <summary>
        ///       http get request that sends the AdminProgram/AddProgram view which displays a form to add a new program to the database
        /// </summary>
        /// <returns>
        ///       a view that contains a submission form for adding a new program
        /// </returns>
        public ActionResult AddProgram() {

            AddProgramViewModel programVM = new AddProgramViewModel();

            List<String> departmentNames = new List<String>();

            foreach (var d in db.Departments.OrderBy(d => d.Name)) { departmentNames.Add(d.Name); }

            programVM.DepartmentNames = departmentNames;

            return View(programVM); }

        // POST: AdminProgram/CreateProgram
        /// <summary>
        ///       http post request that returns the data from the AddProgram submission form in the form of an AddProgramViewModel
        ///       than confirms that the model is valid, if valid the created date and create by fields are added and the hashed and passed into the database
        /// </summary>
        /// <param name="addProgramVM"></param>
        /// <returns>
        ///       returns the user to the list of programs if successful otherwise returns users back to the AddProgram submission form
        /// </returns>
        [HttpPost]
        public ActionResult CreateProgram(AddProgramViewModel programVM)
        {
            if(db.Programs.Where(p=>p.Name==programVM.Program.Name).FirstOrDefault() == null)
            {
                if (ModelState.IsValid)
                {
                    // Adding created on date
                    programVM.Program.CreatedDateTime = DateTime.Now;
                    // Adding created by
                    programVM.Program.CreatedByLoginID = UserData.PersonId;
                    // Adding the new program to the database
                    db.Programs.Add(programVM.Program);
                    db.SaveChanges();
                    Department tempDepartment = new Department();
                    try
                    {
                        //Creating a temp instance of the department matching the selected department
                        foreach(string department in programVM.DepartmentSelection)
                        {
                            tempDepartment = db.Departments.Where(d => d.Name == department).FirstOrDefault();
                            ProgramDepartment pd = new ProgramDepartment();
                            pd.Program = programVM.Program;
                            pd.DepartmentID = tempDepartment.DepartmentID;
                            // Adding created on date
                            pd.CreatedDateTime = DateTime.Now;
                            // Adding created by
                            pd.CreatedByLoginID = UserData.PersonId;
                            db.ProgramDepartments.Add(pd);
                            db.SaveChanges();
                        }
                    }
                    catch { tempDepartment = db.Departments.Where(d => d.DepartmentID == 1).FirstOrDefault(); }
                }
                else
                {
                    //redirects user to the submission form if failed to add school
                    //TODO figure out how to add form errors
                    return RedirectToAction("AddProgram", "AdminProgram");
                }
                //logging that a new program was added
                logger.Info("Program id {Id} added", programVM.Program.ProgramID);
                //redirects user to the list of programs if successfully added new program
                return RedirectToAction("Programs", "AdminProgram", new { page = 1, updatedMessage = "success", addedName = programVM.Program.Name });
            }
            else { return RedirectToAction("Programs", "AdminProgram", new { page = 1, updatedMessage = "error", addedName = programVM.Program.Name }); }
           
        }

        public ActionResult EditProgram(short programID)
        {
            ViewBag.program = db.Programs.Where(p => p.ProgramID == programID).FirstOrDefault();
            var model = new UpdateProgramViewModel
            {
                IsActive = ViewBag.program.IsActive,
                IsSharedProgram = ViewBag.program.IsSharedProgram
            };

            List<String> departmentNames = new List<String>();
            foreach (var d in db.Departments.OrderBy(d => d.Name)) { departmentNames.Add(d.Name); }
            model.DepartmentNames = departmentNames;

            model.DepartmentSelection = db.ProgramDepartments.Join(db.Departments,
                                                                   pd => pd.DepartmentID,
                                                                   d => d.DepartmentID,
                                                                   (pd, d) => new { pd, d })
                                                             .Where(a => a.pd.ProgramID == programID)
                                                             .Select(b => b.d.Name)
                                                             .ToList();

        
                                                  
            return View(model);
        }

        //public ActionResult ViewProgram(short id) { return View(db.Programs.Where(p => p.ProgramID == id).FirstOrDefault()); }

        public ActionResult ViewProgram(int? programID, string updatedMessage)
        {
            var programVM = new ViewProgramViewModel();

            
            programVM.departments = new List<Department>();

            try
            {
                if (programID.HasValue)
                {
                    programVM.program = db.Programs.FirstOrDefault(p => p.ProgramID == programID.Value);
                }

                if (programVM.program.CreatedByLoginID != null)
                {
                    try
                    {
                        programVM.CreatorLogin = (String)db.Users
                            .Where(u => u.PersonID == programVM.program.CreatedByLoginID)
                            .FirstOrDefault()
                            .Login;
                    }
                    catch
                    {
                        programVM.CreatorLogin = "Unknown";
                    }
                }
                if (programVM.program.ModifiedByLoginID != null)
                {
                    try
                    {
                        programVM.ModifierLogin = (String)db.Users
                            .Where(u => u.PersonID == programVM.program.CreatedByLoginID)
                            .FirstOrDefault()
                            .Login;
                    }
                    catch
                    {
                        programVM.ModifierLogin = "Unknown";
                    }
                }

                var programDepartments = db.ProgramDepartments.Where(pd => pd.ProgramID == programID).ToList();

                if (programDepartments !=null)
                {
                    foreach (ProgramDepartment pd in programDepartments) {
                    var tempDepartment = db.Departments.Where(d => d.DepartmentID == pd.DepartmentID).FirstOrDefault();
                    if (tempDepartment != null)
                    {
                        programVM.departments.Add(tempDepartment);
                    }
                }
                }
                if (updatedMessage != null)
                {
                    programVM.updatedMessage = updatedMessage;
                    if (programVM.updatedMessage == "success") { programVM.alertMessage = (programVM.program.Name + " was updated!"); }
                    else { programVM.alertMessage = (programVM.program.Name + " was not updated!"); }
                }


                return View(programVM);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction("Signin", "User");
            }
        }

        [HttpPost]
        public ActionResult UpdateProgram(UpdateProgramViewModel programVM, short programID)
        {
            Program editProgram = db.Programs.Where(p => p.ProgramID == programID).FirstOrDefault();
            List<ProgramDepartment> editProgramDepartment = db.ProgramDepartments.Where(pd => pd.ProgramID == programID).ToList();
            
            IEnumerable<string> tempProgramDepartments = db.ProgramDepartments.Where(pd => pd.ProgramID == programID).Select(pd => pd.Department.Name).ToList();
            IEnumerable<string> toDelete = tempProgramDepartments.Except(programVM.DepartmentSelection);
            IEnumerable<string> toAdd = programVM.DepartmentSelection.Except(tempProgramDepartments);

            if (ModelState.IsValid)
            {
                // Making Program changes
                editProgram.Number = programVM.Program.Number;
                editProgram.Name = programVM.Program.Name;
                editProgram.IsActive = programVM.IsActive;
                editProgram.IsSharedProgram = programVM.IsSharedProgram;
                // Adding modified on date
                editProgram.ModifiedDateTime = DateTime.Now;
                // Adding modified by 
                editProgram.ModifiedByLoginID = UserData.PersonId;

                // Add new ProgramDepartments
                foreach (string departmentName in toAdd)
                {
                    ProgramDepartment newPD = new ProgramDepartment();
                    newPD.ProgramID = programID;
                    newPD.Department = db.Departments.Where(d => d.Name == departmentName).FirstOrDefault();
                    newPD.DepartmentID = db.Departments.Where(d => d.Name == departmentName)
                                                      .Select(d => d.DepartmentID)
                                                      .FirstOrDefault();
                    // Adding created on date
                    newPD.CreatedDateTime = DateTime.Now;
                    // Adding created by
                    newPD.CreatedByLoginID = UserData.PersonId;
                    db.ProgramDepartments.Add(newPD);
                }
               
                // Delete old ProgramDepartments 
                foreach (string department in toDelete)
                {
                    editProgramDepartment.Remove(db.ProgramDepartments.Remove(editProgramDepartment.Where(pd => pd.Department.Name == department).FirstOrDefault()));
                }

                // Modifying the program in the database
                db.SaveChanges();
            } else
            {
                //redirects user to the submission form if failed to add school
                //TODO figure out how to add form errors
                return RedirectToAction("ViewProgram", "AdminProgram", new { programID = programID, updatedMessage = "error" });
            }
            //logging that a the program was modified
            logger.Info("Program id {Id} modified", editProgram.ProgramID);
            //redirects user to the programs view if successfully added new program
            return RedirectToAction("ViewProgram", "AdminProgram", new { programID = programID, updatedMessage = "success" });
        }

        public JsonResult getAllPrograms()
        {
            List<Program> programs = db.Programs.OrderBy(p => p.Name).ToList();
            if (programs.Count() > 0)
            {
                List<ProgramTruncated> allProgramsOutput = new List<ProgramTruncated>();
                foreach (Program p in programs)
                {
                    allProgramsOutput.Add(new ProgramTruncated() { ProgramID = p.ProgramID, Name = p.Name });
                }
                return new JsonResult { Data = allProgramsOutput, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return null;
            }
        }
    }
}