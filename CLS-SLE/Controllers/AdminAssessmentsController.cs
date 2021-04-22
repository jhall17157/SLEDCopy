using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    
    public class AdminAssessmentsController : SLEControllerBase
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();

        // GET: AdminAssessments
        [Authorize(Roles = "Assessments")]
        public ActionResult Index()
        {
            return View();
        }
        private ActionResult Exceptions()
        {
            return RedirectToAction(actionName: "AdminDashboard", controllerName: "Admin");
        }
        [Authorize(Roles = "Assesments")]
        public ActionResult AssessmentMappings()
        {
            return View();
        }

        public ActionResult Assessments()
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.PersonID == UserData.PersonId);
                var adminAssessments = from assessments in db.Assessments
                                           //join permissions in db.AssessmentRubricSecurities on assessments.AssessmentID equals permissions.AssessmentID
                                           //where permissions.PersonID == personID
                                       select assessments;
                logger.Info("Dashboard loaded for " + user.Login);
                var categories = db.AssessmentCategories.ToList();


                dynamic model = new ExpandoObject();

                model.assessments = adminAssessments.Distinct().OrderByDescending(a => a.IsActive).ThenBy(a => a.Name).ToList();
                model.categories = categories;

                return View(model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [Authorize(Roles = "ViewAssesments")]
        public ActionResult ViewAssessment(int? assessmentId)
        {
            var assessment = new Assessment();
            var canEdit = false;
            var canAdd = false;
            try
            {
                if (assessmentId.HasValue)
                {
                    assessment = db.Assessments.FirstOrDefault(a => a.AssessmentID == assessmentId.Value);
                    var permission = db.AssessmentRubricSecurities.FirstOrDefault(p => p.AssessmentID == assessmentId.Value);
                    if (permission != null)
                    {
                        canEdit = permission.CanEdit == 1 ? true : false;
                    }
                }

                if (assessmentId.HasValue)
                {
                    var permission = db.AssessmentRubricSecurities.FirstOrDefault(p => p.AssessmentID == assessmentId.Value);
                    if (permission != null)
                    {
                        canAdd = permission.CanEdit == 1 ? true : false;
                    }
                }

                dynamic model = new ExpandoObject();
                model.CreatorLogin = null;
                model.ModifierLogin = null;

                if (assessment.CreatedByLoginID != null)
                {
                    model.CreatorLogin = (String)db.Users.Where(u => u.PersonID == assessment.CreatedByLoginID).FirstOrDefault().Login;
                }
                if (assessment.ModifiedByLoginID != null)
                {
                    model.ModifierLogin = (String)db.Users.Where(u => u.PersonID == assessment.ModifiedByLoginID).FirstOrDefault().Login;
                }
                model.program = db.Programs.Where(p => p.ProgramID == assessment.ProgramID).FirstOrDefault().Name;
                model.assessment = assessment;
                model.canEdit = canEdit;
                model.canAdd = canAdd;

                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [Authorize(Roles = "EditAssesments")]
        public ActionResult EditAssessment(int? assessmentId)
        {
            var assessment = new Assessment();

            dynamic Model = new ExpandoObject();


            try
            {
                if (assessmentId.HasValue)
                {
                    //var canEdit = false;
                    assessment = db.Assessments.FirstOrDefault(a => a.AssessmentID == assessmentId.Value);
                    //var permission = db.AssessmentRubricSecurities.FirstOrDefault(p => p.AssessmentID == assessmentId.Value);

                    //if (permission != null)
                    //{
                    //    canEdit = permission.CanEdit == 1 ? true : false;
                    //}

                    //if (!canEdit)
                    //{
                    //    throw new Exception("User cannot edit this assessment");
                    //}
                }
                else
                {
                    throw new Exception("Assessment not found!");
                }
                Model.Programs = (from Program in db.Programs
                                  orderby Program.Name
                                  select Program).ToList();

                Model.AssessmentCategories = (from Categories in db.AssessmentCategories
                                              orderby Categories.Name
                                              select Categories).ToList();

                Model.Assessment = assessment;
                return View(Model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return Exceptions();
            }
        }
        [Authorize(Roles = "AddAssesments")]
        [HttpGet]
        public ActionResult AddAssessment(String category)
        {
            dynamic Model = new ExpandoObject();
            Model.Programs = (from programs in db.Programs
                              orderby programs.Name
                              select programs).ToList();

            Model.AssessmentCategories = (from Categories in db.AssessmentCategories
                                          orderby Categories.Name
                                          select Categories).ToList();


            return View(Model);
        }

        [Authorize(Roles = "AddAssesments")]
        //no view was associated with this method below
        [HttpPost]
        public ActionResult InsertNewAssessment(FormCollection formCollection, string category)
        {
            try
            {
                db.Assessments.Load();
                //string Category = formCollection["Category"];
                var CategoryCode = db.AssessmentCategories.Where(c => c.Name == category).FirstOrDefault().CategoryCode;
                var program = (formCollection["Program"]);
                Assessment addAssessment = db.Assessments.Create();


                addAssessment.Name = formCollection["Name"];
                addAssessment.Category = CategoryCode;
                addAssessment.Description = formCollection["Description"] != null ? formCollection["Description"] : "";
                addAssessment.OutcomePassRate = (Decimal?)(Double.Parse(Regex.Replace(formCollection["PassPercent"], "[^0-9.]", ""))) / 100;
                addAssessment.CalculateOutcomePassRate = ((formCollection["CalculateOutcomePassRate"]).Equals("True") ? true : false);
                addAssessment.ProgramID = db.Programs.Where(p => p.Name == program).FirstOrDefault().ProgramID;
                addAssessment.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                addAssessment.CreatedDateTime = DateTime.Now;
                addAssessment.CreatedByLoginID = UserData.PersonId;



                db.Entry(addAssessment).State = EntityState.Added;
                db.SaveChanges();

                return RedirectToAction(actionName: "ViewAssessment", controllerName: "Admin", new { assessmentId = (int)addAssessment.AssessmentID  });

            }
            catch (Exception e)
            {
                logger.Error("Failed to save assessment, redirecting to sign in page.");
                logger.Error(e.Message);
                logger.Error(e.StackTrace);
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [Authorize(Roles = "AddAssesments")]
        //no method was associated with this method below
        [HttpPost]
        public ActionResult SaveAssessment(FormCollection formCollection, short assessmentID)
        {
            try
            {
                // if (Int32.Parse(formCollection["AssessmentID"]) > 0)
                if (assessmentID > 0)
                {
                    // var assessmentid = Int32.Parse(formCollection["AssessmentID"]);
                    var editAssessment = db.Assessments.FirstOrDefault(a => a.AssessmentID == assessmentID);
                    string categoryName = formCollection["Category"];
                    AssessmentCategory category = db.AssessmentCategories.Where(a => a.Name == categoryName).FirstOrDefault();


                    if (editAssessment != null)
                    {
                        editAssessment.Name = formCollection["Name"];
                        editAssessment.Category = category.CategoryCode;
                        editAssessment.Description = formCollection["Description"] != null ? formCollection["Description"] : "";
                        editAssessment.OutcomePassRate = (Decimal?)(Double.Parse(Regex.Replace(formCollection["PassPercent"], "[^0-9.]", ""))) / 100;
                        editAssessment.CalculateOutcomePassRate = ((formCollection["CalculateOutcomePassRate"]).Equals("True") ? true : false);
                        var program = (formCollection["Program"]);
                        editAssessment.ProgramID = db.Programs.Where(p => p.Name == program).FirstOrDefault().ProgramID;
                        editAssessment.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                        editAssessment.ModifiedDateTime = DateTime.Now;
                        editAssessment.ModifiedByLoginID = UserData.PersonId;
                        db.SaveChanges();

                        return RedirectToAction(actionName: "ViewAssessment", controllerName: "Admin", routeValues: new { assessmentId = editAssessment.AssessmentID });
                    }
                    else
                    {
                        logger.Error("Failed to save assessment, redirecting to sign in page.");
                        return RedirectToAction(actionName: "Signin", controllerName: "User");
                    }
                }
                else
                {
                    logger.Error("Failed to save assessment, redirecting to sign in page.");
                    return RedirectToAction(actionName: "Signin", controllerName: "User");
                }
            }
            catch (Exception e)
            {
                logger.Error("Failed to save assessment, redirecting to sign in page.");
                return Content("<html><b>Message:</b><br>" + e.Message + "<br><b>Inner Exception:</b><br>" + e.InnerException + "<br><b>Stack Trace:</b><br>" + e.StackTrace + "</html>");
                // return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }
        
        public ActionResult AssessmentScheduling()
        {
            return View();
        }
       
        public ActionResult ProgramsCourses()
        {
            return View();
        }
        
        public ActionResult SchoolsDepartments()
        {
            return View();
        }

        public ActionResult ViewScoreSet(ScoreSetViewModel scoreSetVM, string message)
        {
            var user = db.Users.FirstOrDefault(u => u.PersonID == UserData.PersonId);
            var sets = db.ScoreSets.ToList();

            scoreSetVM.ScoreSets = sets;
            scoreSetVM.message = message;
            return View(scoreSetVM);
        }
        [HttpPost]
        public ActionResult CreateNewScoreSet(ScoreSetViewModel scoreSetVM)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.PersonID == UserData.PersonId);
                ScoreSet addScoreSet = db.ScoreSets.Create();
                DateTime dateTime = DateTime.Now;

                addScoreSet.Name = scoreSetVM.Name;
                addScoreSet.IsActive = scoreSetVM.IsActive;
                addScoreSet.CreatedDateTime = dateTime;
                addScoreSet.CreatedByLoginID = user.PersonID;

                db.Entry(addScoreSet).State = EntityState.Added;
                db.SaveChanges();

                return RedirectToAction(actionName: "ViewScoreSet", controllerName: "AdminAssessments");

            }
            catch (Exception)
            {

                logger.Error("Failed to save Score Set, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [HttpPost]
        public ActionResult ScoreSetRemoval(byte scoreSetID)
        {
            string msg = "";
            var sets = db.ScoreSets.Where(s => s.ScoreSetID == scoreSetID).FirstOrDefault();
            if (sets.Scores.Count > 0)
            {
                msg = "Please remove all scores from " + sets.Name + " score set before deleting";
            }
            else
            {
                db.ScoreSets.Remove(sets);
                db.SaveChanges();
            }

            return RedirectToAction(actionName: "ViewScoreSet", controllerName: "AdminAssessments", new { message = msg });

        }
        [HttpPost]
        public ActionResult ToggleScoreSetActive(byte scoreSetID)
        {
            var scoreSet = db.ScoreSets.Where(s => s.ScoreSetID == scoreSetID).FirstOrDefault();
            
            if (scoreSet.IsActive){scoreSet.IsActive = false;}
            else{scoreSet.IsActive = true;}

            db.SaveChanges();
            return RedirectToAction(actionName: "ViewScoreSet", controllerName: "AdminAssessments");
        }

        public ActionResult ViewScore(ScoreViewModel scoreVM, byte? ScoreSetID, string message)
        {
            var set = ScoreSetID;
            var scoreList = db.Scores.Where(s => s.ScoreSetID == ScoreSetID).OrderByDescending(s => s.SortOrder).ToList();
            var name = db.ScoreSets.FirstOrDefault(s => s.ScoreSetID == set);

            scoreVM.scores = scoreList;
            scoreVM.ScoreSetName = name.Name;
            scoreVM.ScoreSetID = (byte)ScoreSetID;
            scoreVM.message = message;

            return View(scoreVM);
        }
        [HttpPost]
        public ActionResult ToggleActive(byte scoreID, byte scoreSetID)
        {
            var score = db.Scores.Where(s => s.ScoreID == scoreID).FirstOrDefault();
            //score.IsActive = true ? score.IsActive=false : score.IsActive=true;
            if (score.IsActive){score.IsActive = false;}
            else{score.IsActive = true;}

            db.SaveChanges();

            return RedirectToAction(actionName: "ViewScore", controllerName: "AdminAssessments", new { scoreSetID = score.ScoreSetID });
        }

        [HttpPost]
        public ActionResult CreateNewScore(ScoreViewModel scoreVM, byte scoreSetID)
        {
            var user = db.Users.FirstOrDefault(u => u.PersonID == UserData.PersonId);
            DateTime dateTime = DateTime.Now;
            Score score = new Score();

            score.ScoreSetID = scoreSetID;
            score.IsActive = scoreVM.IsActive;
            score.CreatedByLoginID = user.PersonID;
            score.CreatedDateTime = dateTime;
            score.Description = scoreVM.Description;
            score.Value = (byte)scoreVM.Value;
            score.SortOrder = (byte)scoreVM.SortOrder;

            db.Scores.Add(score);
            db.SaveChanges();

            return RedirectToAction(actionName: "ViewScore", controllerName: "AdminAssessments", new { scoreSetID = score.ScoreSetID }); ;
        }
        [HttpPost]
        public ActionResult RemoveScore(byte scoreId, byte scoreSetID)
        {
            string msg = "";
            var scores = db.Scores.Where(s => s.ScoreID == scoreId).FirstOrDefault();
            var StudentScores = db.StudentScores.Where(s => s.ScoreID == scoreId).ToList();
            if (StudentScores.Count > 0)
            {
                msg = "Cannot delete scores that have been assigned to students";
            }
            else
            {
                db.Scores.Remove(scores);
                db.SaveChanges();
            }
            return RedirectToAction(actionName: "ViewScore", controllerName: "AdminAssessments", new { scoreSetID = scoreSetID, message = msg });
        }



    }
}