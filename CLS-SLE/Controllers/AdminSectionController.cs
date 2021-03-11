using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminSectionController : SLEControllerBase
    {
        //private readonly int PageSize = 20;

        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();

        public ActionResult AddSection(short courseID)
        {
            try
            {
                List<String> leadInstructorList = new List<String>();
                var instructors = db.Users.Where(r => r.UserRoles.Any(ur => ur.Role.Name == "Faculty") && r.IsActive == true);
                foreach(var instructor in instructors)
                {
                    leadInstructorList.Add(instructor.Person.IdNumber + " - " + instructor.Person.FirstName + " " + instructor.Person.LastName);
                };

                
                List<String> semesterList = new List<String>();
                List<Semester> orderedSemesters = new List<Semester>();
                
                orderedSemesters = db.Semesters.OrderByDescending(s => s.SemesterCode).ToList();
                foreach (var semester in orderedSemesters)
                {
                    semesterList.Add(semester.Name);
                }

                var model = new AddSectionViewModel()
                {
                    LeadInstructorList = leadInstructorList,
                    SemesterList = semesterList,
                    SubtermList = db.Subterms.Select(s => s.Name).ToList()
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
                Section editSection = db.Sections.Where(s => s.SectionID == sectionID).FirstOrDefault();
                ViewBag.section = editSection;
                ViewBag.CRN = editSection.CRN.ToString();
                ViewBag.sectionCourse = editSection.Course.CourseName;
                if (editSection.BeginDate != null)
                {
                    ViewBag.BeginDate = editSection.BeginDate.Value.ToString("yyyy-MM-dd");
                }
                if (editSection.EndDate != null)
                {
                    ViewBag.EndDate = editSection.EndDate.Value.ToString("yyyy-MM-dd");
                }
                var leadInstructor = editSection.Person;
                if (leadInstructor != null)
                {
                    ViewBag.LeadInstructor = string.Concat(leadInstructor.IdNumber, " - ", leadInstructor.FirstName, " ", leadInstructor.LastName);
                }

                List<String> leadInstructorList = new List<String>();
                var users = db.Users.ToList();
                var roles = db.Roles.ToList();
                var userroles = db.UserRoles.ToList();
                var query = from ur in userroles

                            join r in roles
                            on ur.RoleID equals r.RoleID into urr
                            from urrResult in (from r in urr
                                               where r.Name == "Faculty"
                                               select r).DefaultIfEmpty()

                            join u in users
                            on ur.PersonID equals u.PersonID into uru
                            from uruResult in uru.DefaultIfEmpty()

                            select new
                            {
                                InstructorName = uruResult != null && uruResult.Person != null ?
                                                uruResult.Person.FirstName + " " + uruResult.Person.LastName
                                                : null,
                                InstructorID = uruResult != null && uruResult.Person != null ?
                                                uruResult.Person.IdNumber : null,
                                PersonRole = urrResult != null ? urrResult.Name : null
                            };
                foreach (var result in query)
                {
                    if (result.PersonRole != null && result.InstructorName != null)
                    {
                        leadInstructorList.Add(string.Concat(result.InstructorID, " - ", result.InstructorName));

                    }
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

                SectionDetailViewModel model = new SectionDetailViewModel();
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
                model.newStudents = new List<StudentModel>();

                model.enrolled = db.Enrollments.Where(e => e.SectionID == sectionID).Where(e => e.EnrollmentStatusCode == "E").OrderBy(e => e.Person.LastName).ToList();
                model.dropped = db.Enrollments.Where(e => e.SectionID == sectionID).Where(e => e.EnrollmentStatusCode != "E").OrderBy(e => e.Person.LastName).ToList();
                return View(model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction("Signin", "User");
            }
        }

        public JsonResult StudentAutoComplete(string search)
        {
            List<StudentSearchModel> resultStudents = db.People.Where(p => (p.IdNumber.Contains(search) || p.LastName.Contains(search))).Select(p => new StudentSearchModel
            {
                idNumber = p.IdNumber,
                lastName = p.LastName,
                firstName = p.FirstName
            }).ToList();

            return new JsonResult { Data = resultStudents, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public ActionResult CreateSection(AddSectionViewModel sectionVM, short courseID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sectionVM.Section.CreatedByLoginID = UserData.PersonId;
                    sectionVM.Section.CreatedDateTime = DateTime.Now;
                    sectionVM.Section.Course = db.Courses
                                                .Where(c => c.CourseName == sectionVM.Section.Course.CourseName)
                                                .FirstOrDefault();
                    if (sectionVM.LeadInstructorSelection != null)
                    {
                        sectionVM.LeadInstructorSelection = sectionVM.LeadInstructorSelection
                                                                    .Substring(0, sectionVM.LeadInstructorSelection.IndexOf("-") - 1);
                        sectionVM.Section.Person = db.People
                                                     .Where(p => p.IdNumber == sectionVM.LeadInstructorSelection)
                                                     .FirstOrDefault();
                    }
                    if (sectionVM.SemesterSelection != null)
                    {
                        sectionVM.Section.Semester = db.Semesters
                                                   .Where(s => s.Name == sectionVM.SemesterSelection)
                                                   .FirstOrDefault();
                    }
                    if (sectionVM.SubtermSelection != null)
                    {
                        sectionVM.Section.Subterm = db.Subterms.Where(s => s.Name == sectionVM.SubtermSelection).FirstOrDefault();
                    }

                    db.Sections.Add(sectionVM.Section);
                    db.SaveChanges();

                    Section createdSection = db.Sections.Where(s => s.CRN == sectionVM.Section.CRN).FirstOrDefault();
                    
                    // When a person is assigned to be the lead instructor of this new section,
                    // this new section should also be added to the section list that this person manages
                    if (createdSection.Person != null)
                    { 
                        Person editPerson = db.People
                                                .Where(p => p.PersonID == sectionVM.Section.Person.PersonID)
                                                .FirstOrDefault();
                        editPerson.Sections.Add(createdSection);
                    }

                    // When a new section is created to be in a semester,
                    // it should also be added to the section list that start in this semester
                    if (createdSection.Semester != null)
                    {
                        Semester editSemester = db.Semesters
                                              .Where(s => s.SemesterID == sectionVM.Section.Semester.SemesterID)
                                              .FirstOrDefault();
                        editSemester.Sections.Add(createdSection);
                    }

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
                if (sectionVM.LeadInstructorSelection != null)
                {
                    sectionVM.LeadInstructorSelection = sectionVM.LeadInstructorSelection
                                                                .Substring(0, sectionVM.LeadInstructorSelection.IndexOf("-") - 1);
                    sectionVM.Section.Person = db.People
                                                 .Where(p => p.IdNumber == sectionVM.LeadInstructorSelection)
                                                 .FirstOrDefault();
                }

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

                        if (editSection.Person != null)
                        {
                            // Remove this section from the previous instructor's list
                            editSection.Person.Sections.Remove(editSection);
                        }

                        // Assign the new person to be the lead instructor
                        editSection.Person = sectionVM.Section.Person;
                    }
                    editSection.BeginDate = sectionVM.Section.BeginDate;
                    editSection.IsCancelled = sectionVM.IsCancelled;
                    editSection.EndDate = sectionVM.Section.EndDate;
                    
                    // Adding modified on date
                    editSection.ModifiedDateTime = DateTime.Now;
                    // Adding modified by 
                    editSection.ModifiedByLoginID = UserData.PersonId;
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

        [HttpPost]
        public ActionResult AddStudent(SectionDetailViewModel studentVM, short sectionID)
        {
            //string studentid = studentVM.newStudent.Substring(studentVM.newStudent.Length-9,9);
            //int id = db.People.Where(p => p.IdNumber == studentid).FirstOrDefault().PersonID;

            int id = db.People.Where(p => p.IdNumber == studentVM.newStudent).FirstOrDefault().PersonID;
            Enrollment newEnrollment = new Enrollment
            {
                SectionID = sectionID,
                StudentID = id,
                EnrollmentStatusCode = "E",
                StatusDate = DateTime.Now,
                CreatedDateTime = DateTime.Now,
                CreatedByLoginID = UserData.PersonId
            };

            db.Enrollments.Add(newEnrollment);
            db.SaveChanges();

            return RedirectToAction("ViewSection", "AdminSection", new { sectionID = sectionID });
        }

        [HttpPost]
        public ActionResult ToggleEnrollmentStatus(int? Id)
        {
            var tempEnrollment = db.Enrollments.Where(e => e.EnrollmentID == Id).FirstOrDefault();
            tempEnrollment.EnrollmentStatusCode = tempEnrollment.EnrollmentStatusCode == "E" ? "D" : "E";
            tempEnrollment.ModifiedByLoginID = UserData.PersonId;
            tempEnrollment.ModifiedDateTime = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("ViewSection", "AdminSection", new { sectionID = tempEnrollment.SectionID });
        }

        public JsonResult StudentListInfo(string search, int id)
        {
            var dataStudent = new StudentModel();
            var student = db.People.Where(p => p.IdNumber == search).FirstOrDefault();

            if (student == null)
            {
                dataStudent.message = "No student has " + search + " as an ID"; dataStudent.success = false;
            }
            else
            {
                if (db.Enrollments.Where(e => e.SectionID == id).Where(e => e.Person.IdNumber == search).FirstOrDefault() == null)
                {
                    dataStudent = new StudentModel { firstName = student.FirstName, lastName = student.LastName, id = student.IdNumber, PID = student.PersonID };
                    dataStudent.success = true;
                }
                else
                {
                    dataStudent.message = "Student "+student.IdNumber+" is already enrolled or dropped in this section.";
                    dataStudent.success = false;
                }
            }
            return new JsonResult { Data = dataStudent, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SaveEnrollment(int id, int section)
        {

            Enrollment newEnrollment = new Enrollment
            {
                SectionID = section,
                StudentID = id,
                EnrollmentStatusCode = "E",
                StatusDate = DateTime.Now,
                CreatedDateTime = DateTime.Now,
                CreatedByLoginID = UserData.PersonId
            };

            db.Enrollments.Add(newEnrollment);
            db.SaveChanges();

            return new JsonResult { Data = id, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult LeadInstructorAutoComplete(string search)
        {
            var instructors = db.Users.Where(r => r.UserRoles.Any(ur => ur.Role.Name == "Faculty"));
            List<LeadInstructorViewModel> resultLeadInsturctors = instructors.Where(p => (p.Person.FirstName.Contains(search) ||
            p.Person.LastName.Contains(search) ||
            p.Person.IdNumber.Contains(search)))
                .Select(p => new LeadInstructorViewModel
                {
                    id = p.Person.IdNumber,
                    name = p.Person.FirstName + " " + p.Person.LastName
                }).ToList();

            return new JsonResult { Data = resultLeadInsturctors, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}