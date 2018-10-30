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
            var instructorAssessments = from x in db.InstructorAssessments select x;
            Session["personID"] = 2;


            return View(instructorAssessments.ToList());
        }

        public ActionResult CLSStudentList(int rubricID)
        {
            var instructor = db.InstructorAssessments.FirstOrDefault(r => r.RubricID == rubricID);

            var students = db.SectionEnrollments.Where(c => c.sectionID == instructor.SectionID).OrderBy(c => c.LastName);

            var assessment = db.InstructorAssessments.Where(a => a.RubricID == rubricID).FirstOrDefault();

            dynamic mymodel = new ExpandoObject();
            mymodel.Students = students.ToList();
            mymodel.Assessment = assessment;


            return View(mymodel);
        }

        public ActionResult TSAStudentList(int rubricID)
        {
            var instructor = db.InstructorAssessments.FirstOrDefault(r => r.RubricID == rubricID);

            var students = db.SectionEnrollments.Where(c => c.sectionID == instructor.SectionID).OrderBy(c => c.LastName);

            var assessment = db.InstructorAssessments.Where(a => a.RubricID == rubricID).FirstOrDefault();

            dynamic mymodel = new ExpandoObject();
            mymodel.Students = students.ToList();
            mymodel.Assessment = assessment;


            return View(mymodel);
        }

        public ActionResult CLSAssessment(int sectionID, int enrollmentID)
        {
            var instructor = db.InstructorAssessments.FirstOrDefault(i => i.SectionID == sectionID);
            Session["instructorID"] = instructor.PersonID;
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

        public ActionResult TSAAssessment(int sectionID, int enrollmentID)
        {
            var instructor = db.InstructorAssessments.FirstOrDefault(i => i.SectionID == sectionID);
            Session["instructorID"] = instructor.PersonID;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssessmentInput(FormCollection fc)
        {
            var outcomeIDs = fc.AllKeys;
            for (var t = 1; t < fc.Count; t++)
            {
                Int16 criteriaID = Convert.ToInt16(outcomeIDs[t]);
                Int16 scoreTypeID = Convert.ToInt16(fc.GetValue(criteriaID.ToString()).AttemptedValue);
                int enrollmentID = Convert.ToInt32(Session["enrollmentID"]);
                
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
                        AssessedByID = Convert.ToInt32(Session["instructorID"]),
                        DateTimeAssessed = DateTime.Now,
                    };
                    db.StudentScores.Add(score);
                }
                db.SaveChanges();
            }

            return RedirectToAction(actionName: "TSAStudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"] });
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
