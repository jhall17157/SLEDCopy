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

        public ActionResult AddSection(short courseID)
        {
            try
            {
                List<String> leadInstructorList = new List<String>();
                foreach (var user in db.People)
                {
                    leadInstructorList.Add(string.Concat(user.FirstName, " ", user.LastName));
                }

                List<String> semesterList = new List<String>();
                foreach (var semester in db.Semesters)
                {
                    semesterList.Add(semester.Name);
                }

                var model = new AddSectionViewModel()
                {
                    LeadInstructorList = leadInstructorList,
                    SemesterList = semesterList
                };

                ViewBag.InitialCourse = db.Courses.Where(c => c.CourseID == courseID).FirstOrDefault();
                ViewBag.CourseID = courseID;
                ViewBag.CourseName = ViewBag.InitialCourse.CourseName;

                return View(model);
            }
            catch
            {
                logger.Error("User attempted to add section without being signed in, redirecting to sign in page.");
                return RedirectToAction("Signin", "User");
            }
        }

        public ActionResult EditSection(short sectionID)
        {
            try
            {
                ViewBag.section = db.Sections.Where(s => s.SectionID == sectionID).FirstOrDefault();
                ViewBag.CRN = ViewBag.section.CRN.ToString();
                ViewBag.sectionCourse = ViewBag.section.Course.CourseName;

                List<String> leadInstructorList = new List<String>();
                foreach (var user in db.People)
                {
                    leadInstructorList.Add(string.Concat(user.FirstName, " ", user.LastName));
                }

                var model = new UpdateSectionViewModel
                {
                    IsCancelled = ViewBag.section.IsCancelled,
                    LeadInstructorList = leadInstructorList
                };

                return View(model);
            }
            catch
            {
                logger.Error("User attempted to edit section without being signed in, redirecting to sign in page.");
                return RedirectToAction("Signin", "User");
            }
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

                if (!section.LeadInstructorID.Equals(null))
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
                            .Where(u => u.PersonID == section.ModifiedByLoginID)
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
        public ActionResult CreateSection(AddSectionViewModel sectionVM, short courseID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sectionVM.Section.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                    sectionVM.Section.CreatedDateTime = DateTime.Now;
                    sectionVM.Section.Person = db.People
                                                 .Where(p => string.Concat(p.FirstName, " ", p.LastName) == sectionVM.LeadInstructorSelection)
                                                 .FirstOrDefault();
                    sectionVM.Section.Semester = db.Semesters
                                                   .Where(s => s.Name == sectionVM.SemesterSelection)
                                                   .FirstOrDefault();
                    sectionVM.Section.Course = db.Courses
                                                .Where(c => c.CourseName == sectionVM.Section.Course.CourseName)
                                                .FirstOrDefault();

                    db.Sections.Add(sectionVM.Section);
                    db.SaveChanges();

                    Section createdSection = db.Sections.Where(s => s.CRN == sectionVM.Section.CRN).FirstOrDefault();

                    // When a person is assigned to be the lead instructor of this new section,
                    // this new section should also be added to the section list that this person manages
                    Person editPerson = db.People
                                            .Where(p => p.PersonID == sectionVM.Section.Person.PersonID)
                                            .FirstOrDefault();
                    editPerson.Sections.Add(createdSection);

                    // When a new section is created to be in a semester,
                    // it should also be added to the section list that start in this semester
                    Semester editSemester = db.Semesters
                                              .Where(s => s.SemesterID == sectionVM.Section.Semester.SemesterID)
                                              .FirstOrDefault();
                    editSemester.Sections.Add(createdSection);

                    // Do the same processes to a course's section list
                    Course editCourse = db.Courses
                                            .Where(c => c.CourseID == sectionVM.Section.Course.CourseID)
                                            .FirstOrDefault();
                    editCourse.Sections.Add(createdSection);
                }
                else
                {
                    //redirects user to the submission form if failed to add section
                    //TODO figure out how to add form errors
                    return RedirectToAction("AddSection", "AdminSection", new { courseID = courseID });
                }
                //logging that a new section was added
                logger.Info("Section id {Id} added", sectionVM.Section.SectionID);
                //redirects user to the list of programs if successfully added new program
                return RedirectToAction("ViewCourse", "AdminCourse", new { courseID = courseID });
            }
            catch
            {
                logger.Error("User attempted to create section without being signed in, redirecting to sign in page.");
                return RedirectToAction("Signin", "User");
            }
        }

        [HttpPost]
        public ActionResult UpdateSection(UpdateSectionViewModel sectionVM, short sectionID)
        {
            try
            {
                Section editSection = db.Sections.Where(s => s.SectionID == sectionID).FirstOrDefault();
                sectionVM.Section.Person = db.People
                                                .Where(p => string.Concat(p.FirstName, " ", p.LastName).Equals(sectionVM.LeadInstructorSelection))
                                                .FirstOrDefault();

                if (ModelState.IsValid)
                {
                    if (editSection.Person != sectionVM.Section.Person &&
                        sectionVM.Section.Person != null)
                    {
                        // When the lead instructor is changed, 
                        // this section needs to be added to the section list of the new instructor
                        // as well as the section list of the previous instructor will no longer
                        // include this section

                        // Add this section to the new instructor's list
                        Person editPerson = db.People.Where(p => p.PersonID == sectionVM.Section.Person.PersonID)
                                                .FirstOrDefault();
                        editPerson.Sections.Add(editSection);

                        // Remove this section from the previous instructor's list
                        editSection.Person.Sections.Remove(editSection);

                        // Assign the new person to be the lead instructor
                        editSection.Person = sectionVM.Section.Person;
                    }
                    editSection.BeginDate = sectionVM.Section.BeginDate;
                    editSection.IsCancelled = sectionVM.IsCancelled;
                    if (editSection.IsCancelled == true)
                    {
                        editSection.EndDate = DateTime.Now;
                    }
                    else
                    {
                        editSection.EndDate = sectionVM.Section.EndDate;
                    }
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
                    return RedirectToAction("EditSection", "AdminSection", new { sectionID = sectionID });
                }
                //logging that the section was updated
                logger.Info("Section id {Id} modified", editSection.SectionID);
                //redirects user to the course view if successfully added new program
                return RedirectToAction("ViewSection", "AdminSection", new { sectionID = editSection.SectionID });
            }
            catch
            {
                logger.Error("User attempted to update section without being signed in, redirecting to sign in page.");
                return RedirectToAction("Signin", "User");
            }
        }
    }
}