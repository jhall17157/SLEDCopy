using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.Models;
using System.Collections.Specialized;

namespace CLS_SLE.Controllers
{
    public class InstructorAssessmentsController : Controller
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();

        // GET: InstructorAssessments
        public ActionResult Dashboard()
        {
            try
            {
                var personID = Convert.ToInt32(Session["personID"].ToString());
                var instructorAssessments = db.InstructorAssessments.Where(i => i.PersonID == personID);

                dynamic model = new ExpandoObject();

                model.assessments = instructorAssessments.ToList();

                return View(model);
            }
            catch
            {
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        public ActionResult StudentList(int rubricID)
        {
            try
            {
                var personID = Convert.ToInt32(Session["personID"].ToString());
                var instructor = db.InstructorAssessments.FirstOrDefault(r => r.RubricID == rubricID && r.PersonID == personID);

                var students = db.SectionEnrollments.Where(c => c.sectionID == instructor.SectionID).OrderBy(c => c.LastName);

                var numCriteria = db.RubricDetails.Count(c => c.RubricID == instructor.RubricID);
                var rubricDetails = db.RubricDetails.Where(r => r.RubricID == rubricID);

                List<float> EnrollmentIDs = new List<float>();
                List<int> CriteriaAnswered = new List<int>();
                foreach(var student in students)
                {
                    var studentScores = db.StudentScores.Where(s => s.EnrollmentID == student.EnrollmentID);
                    var filled = 0;
                    foreach(var score in studentScores)
                    {
                        foreach(var detail in rubricDetails)
                        {
                            if(score.CriteriaID == detail.CriteriaID)
                            {
                                filled++;
                            }
                        }
                    }
                    CriteriaAnswered.Add(filled);

                    EnrollmentIDs.Add(student.EnrollmentID);
                }
                Session["totalScoredFilled"] = CriteriaAnswered;
                Session["assessmentEnrollment"] = EnrollmentIDs;

                var assessment = db.InstructorAssessments.Where(a => a.RubricID == rubricID).FirstOrDefault();

                dynamic mymodel = new ExpandoObject();
                mymodel.Students = students.ToList();
                mymodel.Assessment = assessment;
                mymodel.TotalCriteria = numCriteria;

                return View(mymodel);
            }
            catch 
            {
                return Exceptions();
            }
        }
        
        public ActionResult Assessment(int sectionID, int enrollmentID)
        {
            try
            {
                var personID = Convert.ToInt32(Session["personID"].ToString());
                var instructor = db.InstructorAssessments.FirstOrDefault(i => i.SectionID == sectionID && i.PersonID == personID);
                
                Session["rubricID"] = instructor.RubricID;
                
                var student = db.SectionEnrollments.FirstOrDefault(s => s.sectionID == sectionID && s.EnrollmentID == enrollmentID);
                Session["enrollmentID"] = student.EnrollmentID;

                var rubric = db.InstructorAssessments.FirstOrDefault(n => n.RubricID == instructor.RubricID);

                var outcomes = db.Outcomes.Where(c => c.RubricID == instructor.RubricID);

                var criteria = db.RubricDetails.Where(c => c.RubricID == instructor.RubricID);

                var numberOfSelectors = db.ScoreTypes.Where(n => n.RubricID == instructor.RubricID);

                var studentScores = db.StudentScores.Where(s => s.EnrollmentID == student.EnrollmentID);

                dynamic mymodel = new ExpandoObject();
                mymodel.Student = student;
                mymodel.Rubric = rubric;
                mymodel.Selectors = numberOfSelectors.ToList();
                mymodel.Outcomes = outcomes.ToList();
                mymodel.Criteria = criteria.ToList();
                mymodel.StudentScores = studentScores.ToList();

                return View(mymodel);
            }
            catch
            {
                return Exceptions();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssessmentInput(FormCollection fc)
        {
            var outcomeIDs = fc.AllKeys;
            int enrollmentID = Convert.ToInt32(Session["enrollmentID"]);
            for (var t = 1; t < fc.Count - 1; t++)
            {
                Int16 criteriaID = Convert.ToInt16(outcomeIDs[t]);
                Int16 scoreTypeID = Convert.ToInt16(fc.GetValue(criteriaID.ToString()).AttemptedValue);
                
                var checkIfExists = db.StudentScores.Where(c => c.EnrollmentID == enrollmentID && c.CriteriaID == criteriaID).FirstOrDefault();
                if(checkIfExists != null)
                {
                    checkIfExists.ScoreTypeID = scoreTypeID;
                }
                else
                {
                    StudentScore score = new StudentScore()
                    {
                        EnrollmentID = Convert.ToInt32(Session["enrollmentID"]),
                        CriteriaID = criteriaID,
                        ScoreTypeID = Convert.ToSByte(scoreTypeID),
                        AssessedByID = Convert.ToInt32(Session["personID"]),
                        DateTimeAssessed = DateTime.Now,
                    };
                    db.StudentScores.Add(score);
                }
                db.SaveChanges();
            }
            var submitType = outcomeIDs[outcomeIDs.Count() - 1];

            if(submitType.Equals("nextStudent"))
            {
                return NextStudent();
            }
            else if(submitType.Equals("lastStudent"))
            {
                return LastStudent();
            }
            else if(submitType.Equals("dashboardBreadcrum"))
            {
                return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
            }

            return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"] });
        }

        private ActionResult NextStudent()
        {
            var enrollmentID = Convert.ToInt32(Session["enrollmentID"]);
            var enrollment = db.SectionEnrollments.FirstOrDefault(e => e.EnrollmentID == enrollmentID);
            var list = (List<float>)Session["assessmentEnrollment"];
            for (var x = 0; x < list.Count; x++)
            {
                if (enrollmentID == list[x])
                {
                    try
                    {
                        return RedirectToAction(actionName: "Assessment", controllerName: "InstructorAssessments", routeValues: new { sectionID = enrollment.sectionID, enrollmentID = list[x+1] });
                    }
                    catch
                    {
                        return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"] });
                    }
                }
            }
            return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"] });
        }

        private ActionResult LastStudent()
        {
            var enrollmentID = Convert.ToInt32(Session["enrollmentID"]);
            var enrollment = db.SectionEnrollments.FirstOrDefault(e => e.EnrollmentID == enrollmentID);
            var list = (List<float>)Session["assessmentEnrollment"];
            for (var x = 0; x < list.Count; x++)
            {
                if (enrollmentID == list[x])
                {
                    try
                    {
                        return RedirectToAction(actionName: "Assessment", controllerName: "InstructorAssessments", routeValues: new { sectionID = enrollment.sectionID, enrollmentID = list[x - 1] });
                    }
                    catch
                    {
                        return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"] });
                    }
                }
            }
            return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"] });
        }

        private ActionResult Exceptions()
        {
            return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
