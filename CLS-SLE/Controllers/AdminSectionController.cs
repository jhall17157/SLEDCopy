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
    public class AdminSectionController : Controller
    {
        //private readonly int PageSize = 20;

        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult EditSection(short sectionID)
        {
            ViewBag.section = db.Sections.Where(s => s.SectionID == sectionID).FirstOrDefault();
            ViewBag.CRN = ViewBag.section.CRN.ToString();
            List<String> semesterNames = new List<String>();
            foreach (var semester in db.Semesters)
            {
                semesterNames.Add(semester.Name);
            }
            var model = new UpdateSectionViewModel
            {
                IsCancelled = ViewBag.section.IsCancelled,
                SemesterNames = semesterNames,
                Section = db.Sections.Where(s => s.SectionID == sectionID).FirstOrDefault()
            };

            return View(model);
        }

        public ActionResult ViewSection(int? sectionID)
        {
            var section = new Section();
            var leadInstructorFName = "";
            var leadInstructorLName = "";
            
            try
            {
                if (sectionID.HasValue)
                {
                    section = db.Sections.FirstOrDefault(s => s.SectionID == sectionID.Value);
                }

                dynamic model = new ExpandoObject();
                model.CreatorLogin = null;
                model.ModifierLogin = null;

                if (section.LeadInstructorID != null)
                {
                    try
                    {
                        leadInstructorFName = (String)db.People
                            .Where(p => p.PersonID == section.LeadInstructorID)
                            .FirstOrDefault()
                            .FirstName;
                        leadInstructorLName = (String)db.People
                            .Where(p => p.PersonID == section.LeadInstructorID)
                            .FirstOrDefault()
                            .LastName;
                        model.sectionLeadInstructor = string.Concat(leadInstructorFName, " ", leadInstructorLName);
                    }
                    catch
                    {
                        model.sectionLeadInstructor = "Unknown";
                    }
                }

                if (section.CreatedByLoginID != null)
                {
                    try
                    {
                        model.CreatorLogin = (String)db.Users
                            .Where(u => u.PersonID == section.CreatedByLoginID)
                            .FirstOrDefault()
                            .Login;
                    }
                    catch
                    {
                        model.CreatorLogin = "Unknown";
                    }
                }
                if (section.ModifiedByLoginID != null)
                {
                    try
                    {
                        model.ModifierLogin = (String)db.Users
                            .Where(u => u.PersonID == section.CreatedByLoginID)
                            .FirstOrDefault()
                            .Login;
                    }
                    catch
                    {
                        model.ModifierLogin = "Unknown";
                    }
                }
                model.section = section;
                model.sectionSemester = (String)db.Semesters
                            .Where(s => s.SemesterID == section.SemesterID)
                            .FirstOrDefault()
                            .Name;
                model.sectionCourse = (String)db.Courses
                            .Where(c => c.CourseID == section.CourseID)
                            .FirstOrDefault()
                            .CourseName;

                return View(model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction("Signin", "User");
            }
        }

        [HttpPost]
        public ActionResult UpdateSection(UpdateSectionViewModel sectionVM, short sectionID)
        {
            Section editSection = db.Sections.Where(s => s.SectionID == sectionID).FirstOrDefault();

            if (ModelState.IsValid)
            {
                editSection.CRN = sectionVM.Section.CRN;
                //editSection.SemesterID = sectionVM.Section.SemesterID;
                if(editSection.Semester != sectionVM.Section.Semester)
                {
                    Semester editSemester = db.Semesters.Where(s => s.SemesterID == sectionVM.Section.SemesterID).FirstOrDefault();

                    editSemester.Sections.Add(sectionVM.Section);
                    editSection.Semester = sectionVM.Section.Semester;
                }
                editSection.CourseID = sectionVM.Section.CourseID;
                editSection.LeadInstructorID = sectionVM.Section.LeadInstructorID;
                editSection.OfferingNumber = sectionVM.Section.OfferingNumber;
                editSection.IsCancelled = sectionVM.IsCancelled;
                editSection.BeginDate = sectionVM.Section.BeginDate;
                editSection.EndDate = sectionVM.Section.EndDate;
                // Adding modified on date
                editSection.ModifiedDateTime = DateTime.Now;
                // Adding modified by 
                editSection.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                // Modifying the program in the database
                db.SaveChanges();
            }
            else
            {
                //redirects user to the submission form if failed to update section
                //TODO figure out how to add form errors
                return RedirectToAction("AddProgram", "AdminProgram");
            }
            //logging that the section was updated
            logger.Info("Section id {Id} modified", editSection.SectionID);
            //redirects user to the course view if successfully added new program
            return RedirectToAction("ViewCourse", "AdminCourse", new { id = editSection.CourseID });
        }
    }
}