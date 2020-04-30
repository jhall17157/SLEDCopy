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
    public class AdminProgramController : Controller
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
        public ActionResult Programs(int page) => View(new ProgramListViewModel
        {
            Programs = db.Programs
                            .OrderBy(p => p.Name)
                            .Skip((page - 1)*PageSize)
                            .Take(PageSize),
            PagingInfo = new PagingInfo { CurrentPage = page,
                                            ItemsPerPage = PageSize,
                                            TotalItems = db.Programs.Count() }
        });

        // GET: AdminProgram/AddProgram
        /// <summary>
        ///       http get request that sends the AdminProgram/AddProgram view which displays a form to add a new program to the database
        /// </summary>
        /// <returns>
        ///       a view that contains a submission form for adding a new program
        /// </returns>
        public ActionResult AddProgram() { return View(); }

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
            if (ModelState.IsValid)
            {
                // Adding created on date
                programVM.Program.CreatedDateTime = DateTime.Now;
                // Adding created by
                programVM.Program.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                // Adding the new program to the database
                db.Programs.Add(programVM.Program);
                db.SaveChanges();
            } else
            {
                //redirects user to the submission form if failed to add school
                //TODO figure out how to add form errors
                return RedirectToAction("AddProgram", "AdminProgram");
            }
            //logging that a new program was added
            logger.Info("Program id {Id} added", programVM.Program.ProgramID);
            //redirects user to the list of programs if successfully added new program
            return RedirectToAction("Programs", "AdminProgram", new { page = 1 });
        }

        public ActionResult EditProgram(short programID)
        {
            ViewBag.program = db.Programs.Where(p => p.ProgramID == programID).FirstOrDefault();
            var model = new UpdateProgramViewModel
            {
                IsActive = ViewBag.program.IsActive,
                IsSharedProgram = ViewBag.program.IsSharedProgram
            };
            return View(model);
        }

        //public ActionResult ViewProgram(short id) { return View(db.Programs.Where(p => p.ProgramID == id).FirstOrDefault()); }

        public ActionResult ViewProgram(int? programID)
        {
            var program = new Program();
            //var canEdit = false;

            try
            {
                if (programID.HasValue)
                {
                    program = db.Programs.FirstOrDefault(p => p.ProgramID == programID.Value);
                    /*var permission = db.ProgramSecurities.FirstOrDefault(p => p.ProgramID == programID.Value);
                    if (permission != null)
                    {
                        canEdit = permission.CanEdit == true ? true : false;
                    }*/
                }

                dynamic model = new ExpandoObject();
                model.CreatorLogin = null;
                model.ModifierLogin = null;

                if (program.CreatedByLoginID != null)
                {
                    try
                    {
                        model.CreatorLogin = (String)db.Users
                            .Where(u => u.PersonID == program.CreatedByLoginID)
                            .FirstOrDefault()
                            .Login;
                    }
                    catch
                    {
                        model.CreatorLogin = "Unknown";
                    }
                }
                if (program.ModifiedByLoginID != null)
                {
                    try
                    {
                        model.ModifierLogin = (String)db.Users
                            .Where(u => u.PersonID == program.CreatedByLoginID)
                            .FirstOrDefault()
                            .Login;
                    }
                    catch
                    {
                        model.ModifierLogin = "Unknown";
                    }
                }
                model.program = program;
                //model.canEdit = canEdit;

                return View(model);
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

            if (ModelState.IsValid)
            {
                editProgram.Number = programVM.Program.Number;
                editProgram.Name = programVM.Program.Name;
                editProgram.IsActive = programVM.IsActive;
                editProgram.IsSharedProgram = programVM.IsSharedProgram;
                // Adding modified on date
                editProgram.ModifiedDateTime = DateTime.Now;
                // Adding modified by 
                editProgram.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                // Modifying the program in the database
                db.SaveChanges();
            } else
            {
                //redirects user to the submission form if failed to add school
                //TODO figure out how to add form errors
                return RedirectToAction("Programs", "AdminProgram", new { page = 1 });
            }
            //logging that a new program was added
            logger.Info("Program id {Id} modified", editProgram.ProgramID);
            //redirects user to the programs view if successfully added new program
            return RedirectToAction("Programs", "AdminProgram", new { page = 1 });
        }
    }
}