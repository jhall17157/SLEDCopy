using CLS_SLE.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class InstructorAssessmentsController : SLEControllerBase
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();

        // GET: InstructorAssessments
        public ActionResult Dashboard()
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.PersonID == UserData.PersonId);
                var instructorAssessments = db.InstructorAssessments.Where(i => i.PersonID == UserData.PersonId);
                logger.Info("Dashboard loaded for " + user.Login);
                var categories = db.AssessmentCategories.ToList();

                dynamic model = new ExpandoObject();

                model.assessments = instructorAssessments.ToList();
                model.categories = categories;

                List<AssessmentLevelPair> assessmentLevelPairs = new List<AssessmentLevelPair>();


                foreach (InstructorAssessment assessment in instructorAssessments)
                {
                    if (assessment.AssessmentLevel != null)
                    {
                        assessmentLevelPairs.Add(new AssessmentLevelPair(assessment, db.AssessmentLevels.Where(l => l.AssessmentLevelCode == assessment.AssessmentLevel).FirstOrDefault().Name));
                    }
                    else
                    {
                        assessmentLevelPairs.Add(new AssessmentLevelPair(assessment, null));
                    }
                }
                model.assessmentLevelPairs = assessmentLevelPairs;



                return View(model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }


        public ActionResult StudentList(int rubricID, int sectionID)
        {
            try
            {
                var instructor = db.InstructorAssessments.FirstOrDefault(r => r.RubricID == rubricID && r.PersonID == UserData.PersonId && r.SectionID == sectionID);
                logger.Info("Assessment student list loaded for " + instructor.Login + " with rubricID " + rubricID);

                var students = db.SectionEnrollments.Where(c => c.sectionID == instructor.SectionID).OrderBy(c => c.LastName).ThenBy(c => c.FirstName);

                Session["assessmentEnrollment"] = students.Select(u => u.EnrollmentID).ToList();

                var completedScores = db.StudentScoreCounts.Where(c => c.RubricID == rubricID && c.SectionID == instructor.SectionID);

                var assessment = db.InstructorAssessments.Where(a => a.RubricID == rubricID && a.SectionID == instructor.SectionID).FirstOrDefault();

                dynamic mymodel = new ExpandoObject();
                mymodel.Students = students.ToList();
                mymodel.Assessment = assessment;
                mymodel.CompleteScores = completedScores.ToList();

                if (assessment.AssessmentLevel != null)
                {
                    var level = db.AssessmentLevels.Where(l => l.AssessmentLevelCode == assessment.AssessmentLevel).FirstOrDefault().Name;
                    mymodel.Level = level;
                }



                return View(mymodel);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return Exceptions();
            }
        }


        public ActionResult Assessment(int sectionID, int enrollmentID, int rubricID)
        {
            try
            {
                var instructor = db.InstructorAssessments.FirstOrDefault(i => i.SectionID == sectionID && i.PersonID == UserData.PersonId && i.RubricID == rubricID);
                
                Session["rubricID"] = instructor.RubricID;
                Session["sectionID"] = instructor.SectionID;

                var ScoreSet = db.AssessmentRubrics.FirstOrDefault(n => n.RubricID == rubricID);

                var student = db.SectionEnrollments.FirstOrDefault(s => s.sectionID == sectionID && s.EnrollmentID == enrollmentID);
                Session["enrollmentID"] = student.EnrollmentID;

                logger.Info("Assessment loaded for student " + student.FirstName + " by " + instructor.Login + " in assesssment with rubricID " + rubricID);
                var rubric = db.InstructorAssessments.FirstOrDefault(n => n.RubricID == instructor.RubricID && n.SectionID == instructor.SectionID);
                var outcomes = db.Outcomes.Where(c => c.RubricID == instructor.RubricID && c.IsActive == true);
                var criteria = db.RubricDetails.Where(c => c.RubricID == instructor.RubricID);
                var numberOfSelectors = db.Scores.Where(n => n.ScoreSetID == ScoreSet.ScoreSetID && n.IsActive == true).OrderBy(n => n.SortOrder);
                var studentScores = db.StudentScores.Where(s => s.EnrollmentID == student.EnrollmentID);

                dynamic mymodel = new ExpandoObject();
                mymodel.Student = student;
                mymodel.Rubric = rubric;
                mymodel.Selectors = numberOfSelectors.ToList();
                mymodel.Outcomes = outcomes.ToList();
                mymodel.Criteria = criteria.ToList();
                mymodel.StudentScores = studentScores.ToList();
                mymodel.ScoreSetID = ScoreSet.ScoreSetID;

                return View(mymodel);
            }
            catch
            {
                logger.Error("User attempted to load assessment without being signed in, redirecting to sign in page.");
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
                try
                {
                    Int16 criteriaID = Convert.ToInt16(outcomeIDs[t]);
                    Int16 scoreID = Convert.ToInt16(fc.GetValue(criteriaID.ToString()).AttemptedValue);
                    var checkIfExists = db.StudentScores.Where(c => c.EnrollmentID == enrollmentID && c.CriteriaID == criteriaID).FirstOrDefault();
                    if (checkIfExists != null)
                    {
                        checkIfExists.ScoreID = scoreID;
                    }
                    else
                    {
                        StudentScore score = new StudentScore()
                        {
                            EnrollmentID = Convert.ToInt32(Session["enrollmentID"]),
                            CriteriaID = criteriaID,
                            ScoreID = Convert.ToSByte(scoreID),
                            AssessedByID = UserData.PersonId,
                            DateTimeAssessed = DateTime.Now,
                        };
                        db.StudentScores.Add(score);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    //do nothing - if the user hits the next button too fast, it fills in additional items into the array that are not scores
                    //kind of a hack fix but it works
                }
            }
            var submitType = outcomeIDs[outcomeIDs.Count() - 1];


            if (submitType.Equals("nextStudent"))
            {
                logger.Info("Submission recieved and saved, loading data for next student");
                return NextStudent();
            }
            else if (submitType.Equals("lastStudent"))
            {
                logger.Info("Submission recieved and saved, loading data for previous student");
                return LastStudent();
            }
            else if (submitType.Equals("dashboardBreadcrum"))
            {
                logger.Info("Submission recieved and saved, redirecting to dashboard");
                return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
            }
            else if (submitType.Equals("studentListBreadcrum"))
            {
                logger.Info("Submission recieved and saved, redirecting to student list");
                return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"], sectionID = Session["sectionID"] });
            }
            else if (submitType.Equals("here"))
            {
                logger.Info("Submission recieved and saved, returning to here");
                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            else if (submitType.Equals("backToTop"))
            {
                logger.Info("Submission recieved and saved, returning to the list of students");
                return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"], sectionID = Session["sectionID"] });
            }
            logger.Info("Submission recieved and saved, redirecting to assessment student list");
            return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"], sectionID = Session["sectionID"] });
        }

        private ActionResult NextStudent()
        {
            var enrollmentID = Convert.ToInt32(Session["enrollmentID"]);
            var enrollment = db.SectionEnrollments.FirstOrDefault(e => e.EnrollmentID == enrollmentID);
            var list = (List<long>)Session["assessmentEnrollment"];
            for (var x = 0; x < list.Count; x++)
            {
                if (enrollmentID == list[x])
                {
                    try
                    {
                        return RedirectToAction(actionName: "Assessment", controllerName: "InstructorAssessments", routeValues: new { sectionID = enrollment.sectionID, enrollmentID = list[x + 1], rubricID = Session["rubricID"] });
                    }
                    catch
                    {
                        return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"], sectionID = Session["sectionID"] });
                    }
                }
            }
            return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"], sectionID = Session["sectionID"] });
        }

        private ActionResult LastStudent()
        {
            var enrollmentID = Convert.ToInt32(Session["enrollmentID"]);
            var enrollment = db.SectionEnrollments.FirstOrDefault(e => e.EnrollmentID == enrollmentID);
            var list = (List<long>)Session["assessmentEnrollment"];
            for (var x = 0; x < list.Count; x++)
            {
                if (enrollmentID == list[x])
                {
                    try
                    {
                        return RedirectToAction(actionName: "Assessment", controllerName: "InstructorAssessments", routeValues: new { sectionID = enrollment.sectionID, enrollmentID = list[x - 1], rubricID = Session["rubricID"] });
                    }
                    catch
                    {
                        return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"], sectionID = Session["sectionID"] });
                    }
                }
            }
            return RedirectToAction(actionName: "StudentList", controllerName: "InstructorAssessments", routeValues: new { rubricID = Session["rubricID"], sectionID = Session["sectionID"] });
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
