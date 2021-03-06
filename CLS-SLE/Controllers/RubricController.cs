using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles = "ViewRubrics")]

    public class RubricController : SLEControllerBase
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        // GET: Rubric

        [Authorize(Roles = "ViewRubrics")]
        public ActionResult Index()
        {
            dynamic Model = new ExpandoObject();

            List<AssessmentRubric> Rubrics = (from Rubric in db.AssessmentRubrics
                                              select Rubric).ToList();
            Model.Rubrics = Rubrics;
            return View(Model);
        }

        [Authorize(Roles = "ViewRubrics")]
        public ActionResult ViewRubric(int? rubricID, short? assessmentID)
        {
            dynamic Model = new ExpandoObject();
            var Rubric = db.RubricAssessments.Where(r => r.RubricID == rubricID && r.AssessmentID == assessmentID).FirstOrDefault();
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> Logins = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            Logins.Add("Outcome", new Dictionary<string, Dictionary<string, string>>());
            Logins.Add("Criterion", new Dictionary<string, Dictionary<string, string>>());
            Model.Outcomes = db.Outcomes.Where(o => o.RubricID == rubricID).ToList();
            foreach (Outcome outcome in Model.Outcomes)
            {
                string outcomeID = outcome.OutcomeID.ToString();

                Logins["Outcome"].Add(outcomeID, new Dictionary<string, string>());

                if (outcome.ModifiedByLoginID != null)
                {
                    Logins["Outcome"][outcomeID].Add("ModifierLogin", db.Users.Where(u => u.PersonID == outcome.ModifiedByLoginID).FirstOrDefault().Login.ToString());
                }

                if (outcome.CreatedByLoginID != null)
                {
                    Logins["Outcome"][outcomeID].Add("CreatorLogin", db.Users.Where(u => u.PersonID == outcome.CreatedByLoginID).FirstOrDefault().Login.ToString());
                }

                foreach (Criterion criterion in outcome.Criteria)
                {
                    string criteriaID = criterion.CriteriaID.ToString();

                    Logins["Criterion"].Add(criteriaID, new Dictionary<string, string>());

                    if (criterion.ModifiedByLoginID != null)
                    {
                        Logins["Criterion"][criteriaID].Add("ModifierLogin", db.Users.Where(u => u.PersonID == criterion.ModifiedByLoginID).FirstOrDefault().Login.ToString());
                    }

                    if (criterion.CreatedByLoginID != null)
                    {
                        Logins["Criterion"][criteriaID].Add("CreatorLogin", db.Users.Where(u => u.PersonID == criterion.CreatedByLoginID).FirstOrDefault().Login.ToString());
                    }
                }
            }
            Model.Logins = Logins;
            Model.Rubric = Rubric;
            Model.CreatorLogin = null;
            Model.ModifierLogin = null;

            //get rubrics under the same assesment
            List<RubricSearchModel> resultRubrics = db.RubricAssessments.Where(a => a.AssessmentID == assessmentID).Select(a => new RubricSearchModel
            {
                name = a.AssessmentRubric.Name,
                rubricID = a.RubricID
            }).ToList();

            ViewBag.rubricsInAssessments = resultRubrics;

            return View(Model);
        }
        [Authorize(Roles = "AddRubrics")]
        [HttpGet]
        public ActionResult AddRubric(short? assessmentID)
        {
            string category = db.Assessments.Where(a => a.AssessmentID == assessmentID).Select(a => a.Category).FirstOrDefault();
            ViewBag.Assessments = db.Assessments.Where(a => a.Category == category).ToList();
            ViewBag.ScoreSets = db.ScoreSets.ToList();
            ViewBag.InitialAssessmentID = assessmentID;

            return View();
        }

        //post for submitting a new Rubric
        [Authorize(Roles = "AddRubrics")]
        [HttpPost]
        public ActionResult InsertNewRubric(RubricViewModel rubricViewModel, FormCollection formCollection)
        {
            try
            {
                // RubricAssessment rubricAssessment = rubricViewModel.RubricAssesssment;
                // AssessmentRubric assessmentRubric = rubricViewModel.AssessmentRubric;
                List<Assessment> assessments = new List<Assessment>();

                foreach (var key in formCollection.AllKeys)
                    if (key.Contains("relatedAssessment"))
                    {
                        int id = Convert.ToInt32(formCollection[key]);
                        assessments.Add(db.Assessments.Where(a => a.AssessmentID == id ).FirstOrDefault());
                    }
                   
                db.AssessmentRubrics.Load();

                rubricViewModel.AssessmentRubric.CreatedDateTime = DateTime.Now;
                rubricViewModel.AssessmentRubric.CreatedByLoginID = UserData.PersonId;

                db.AssessmentRubrics.Add(rubricViewModel.AssessmentRubric);

                db.RubricAssessments.Load();

                foreach (Assessment assessment in assessments)
                {
                    RubricAssessment rubricAssessment = new RubricAssessment();

                    rubricAssessment.AssessmentID = assessment.AssessmentID;
                    rubricAssessment.RubricID = rubricViewModel.RubricAssesssment.RubricID;
                    rubricAssessment.StartDate = rubricViewModel.RubricAssesssment.StartDate;
                    rubricAssessment.EndDate = rubricViewModel.RubricAssesssment.EndDate;
                    rubricAssessment.CreatedDateTime = DateTime.Now;
                    rubricAssessment.CreatedByLoginID = UserData.PersonId;

                    db.RubricAssessments.Add(rubricAssessment);
                }

                db.SaveChanges();

                return RedirectToAction("ViewRubric", "Rubric", new { rubricID = rubricViewModel.AssessmentRubric.RubricID, assessmentID = rubricViewModel.AssessmentID });
            }
            catch (Exception e)
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [Authorize(Roles = "EditRubrics")]
        [HttpGet]
        public ActionResult EditRubric(short rubricID, short assessmentID)
        {
            RubricAssessment rubricAssessment = db.RubricAssessments.Where(r => r.RubricID == rubricID && r.AssessmentID == assessmentID).FirstOrDefault();
            AssessmentRubric assessmentRubric = db.AssessmentRubrics.Where(a => a.RubricID == rubricID).FirstOrDefault();
            List<short> relatedAssessmentIDs = db.RubricAssessments.Where(r => r.RubricID == rubricID).Select(r => r.AssessmentID).ToList();
            // List<string> relatedAssessments = new List<string>();

            // foreach (short relatedAssessmentID in relatedAssessmentIDs)
            //    relatedAssessments.Add(db.Assessments.Where(a => a.AssessmentID == relatedAssessmentID).FirstOrDefault().Name);

            string category = db.Assessments.Where(a => a.AssessmentID == assessmentID).Select(a => a.Category).FirstOrDefault();
            ViewBag.Assessments = db.Assessments.Where(a => a.Category == category).ToList();

            ViewBag.RelatedAssessmentIDs = relatedAssessmentIDs;
            ViewBag.ScoreSets = db.ScoreSets.ToList();

            var model = new UpdateRubric() { RubricAssessment = rubricAssessment, AssessmentRubric = assessmentRubric };

            return View(model);
        }

        //Post for saving an edited rubric
        [Authorize(Roles = "EditRubrics")]
        [HttpPost]
        public ActionResult SaveRubric(UpdateRubric updateRubric, short rubricID, FormCollection formCollection, short assessmentID)
        {
            try
            {
                AssessmentRubric editRubric = db.AssessmentRubrics.Where(r => r.RubricID == rubricID).FirstOrDefault();
                List<short> dbRelatedAssessmentIDs = db.RubricAssessments.Where(r => r.RubricID == rubricID).Select(r => r.AssessmentID).ToList();
                List<short> editRelatedAssessmentIDs = new List<short>();

                foreach (var key in formCollection.AllKeys)
                    if (key.Contains("relatedAssessment"))
                    {
                        short id = Convert.ToInt16(formCollection[key]);
                        editRelatedAssessmentIDs.Add(id);
                    }

                    db.RubricAssessments.Load();

                //if a related assessment got deleted, delete it in the db
                foreach (short dbRelatedID in dbRelatedAssessmentIDs)
                    if (editRelatedAssessmentIDs.IndexOf(dbRelatedID) == -1)
                    {
                        db.RubricAssessments.Remove(db.RubricAssessments.Where(r => r.AssessmentID == dbRelatedID && r.RubricID == rubricID).FirstOrDefault());
                    }

                // add new related assessments
                foreach (short editRelatedAssessmentID in editRelatedAssessmentIDs)
                    if (dbRelatedAssessmentIDs.IndexOf(editRelatedAssessmentID) == -1)
                    {

                        RubricAssessment rubricAssessment = new RubricAssessment();

                        rubricAssessment.AssessmentID = editRelatedAssessmentID;
                        rubricAssessment.RubricID = rubricID;
                        rubricAssessment.StartDate = DateTime.Now;//set to the current date time for now
                        rubricAssessment.EndDate = null;//set to null for now
                        rubricAssessment.CreatedDateTime = DateTime.Now;
                        rubricAssessment.CreatedByLoginID = UserData.PersonId;

                        db.RubricAssessments.Add(rubricAssessment);
                    }

                AssessmentRubric UpdateRubric = db.AssessmentRubrics.Where(r => r.RubricID == rubricID).FirstOrDefault();

                UpdateRubric.Name = updateRubric.AssessmentRubric.Name;
                UpdateRubric.Description = updateRubric.AssessmentRubric.Description;
                UpdateRubric.IsActive = updateRubric.AssessmentRubric.IsActive;
                UpdateRubric.ModifiedDateTime = DateTime.Now;
                UpdateRubric.ModifiedByLoginID = UserData.PersonId;
                UpdateRubric.ScoreSetID = updateRubric.AssessmentRubric.ScoreSetID;
                                
                db.SaveChanges();

                return RedirectToAction("ViewRubric", "Rubric", new { rubricID, assessmentID = editRelatedAssessmentIDs.Contains(assessmentID) ? assessmentID : editRelatedAssessmentIDs[0] });
            }
            catch (Exception e)
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [Authorize(Roles = "AddOutcomesCriteria")]
        [HttpGet]
        public ActionResult AddOutcome(short rubricID, short assessmentID)
        {
            RubricAssessment rubric = db.RubricAssessments.Where(r => r.RubricID == rubricID && r.AssessmentID == assessmentID).FirstOrDefault();
            OutcomeViewModel model = new OutcomeViewModel() { OutcomeVM = new Outcome() { RubricID = rubricID }, Rubric = rubric };
            //Defaulting form
            model.OutcomeVM.IsActive = true;
            model.OutcomeVM.CalculateCriteriaPassRate = true;
            model.OutcomeVM.IsTSAOutcome = true;
            return View(model);
        }

        [Authorize(Roles = "AddOutcomesCriteria")]
        [HttpPost]
        public ActionResult InsertNewOutcome(OutcomeViewModel outcomeViewModel)
        {
            try
            {
                short rubricID = outcomeViewModel.Rubric.RubricID;
                Byte maxSortOrder = 0;
                if (db.Outcomes.Where(c => c.RubricID == rubricID).Any())
                {
                    maxSortOrder = db.Outcomes.Where(c => c.RubricID == rubricID).OrderByDescending(r => r.SortOrder).FirstOrDefault().SortOrder;
                }
                maxSortOrder++;

                AssessmentRubric rubric = db.AssessmentRubrics.Where(a => a.RubricID == rubricID).FirstOrDefault();
                outcomeViewModel.OutcomeVM.SortOrder = maxSortOrder;
                outcomeViewModel.OutcomeVM.CriteriaPassRate = outcomeViewModel.OutcomeVM.CriteriaPassRate / 100;
                outcomeViewModel.OutcomeVM.CreatedDateTime = DateTime.Now;
                outcomeViewModel.OutcomeVM.CreatedByLoginID = UserData.PersonId;
                rubric.Outcomes.Add(outcomeViewModel.OutcomeVM);
                db.Outcomes.Add(outcomeViewModel.OutcomeVM);
                db.SaveChanges();

                return RedirectToAction("ViewRubric", "Rubric", new { rubricID = outcomeViewModel.Rubric.RubricID, assessmentID = outcomeViewModel.Rubric.AssessmentID });
            }
            catch (Exception e)
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return Content("<html><b>Message:</b><br>" + e.Message + "<br><b>Inner Exception:</b><br>" + e.InnerException + "<br><b>Stack Trace:</b><br>" + e.StackTrace + "</html>");
               // return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }


        [Authorize(Roles = "EditOutcomesCriteria")]
        [HttpGet]
        public ActionResult EditOutcome(short outcomeID, short assessmentID)
        {
            Outcome outcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();
            RubricAssessment rubric = db.RubricAssessments.Where(r => r.RubricID == outcome.RubricID && r.AssessmentID == assessmentID).FirstOrDefault();
            var model = new OutcomeViewModel() { OutcomeVM = outcome, Rubric = rubric };

            return View(model);
        }

        /*

        public ActionResult EditOutcome(int? outcomeID, int? rubricID)
		{
			dynamic Model = new ExpandoObject();
			Model.Outcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();
			return View(Model);
		}

        /*
         [HttpPost]
         public ActionResult SaveOutcome(int outcomeID, rubricID, UpdateOutcome updateOutcome)
        {
            try
            {
                db.Outcomes.Load();
                Outcome editOutcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();
                AssessmentRubric editRubric = db.AssessmentRubrics.Where(r => r.RubricID == rubricID).FirstOrDefault();

                editOutcome.RubricID = rubricID;
                editOutcome.Name = updateOutcome.Outcome.Name;
                editOutcome.Description = updateOutcome.Outcome.Description
                editOutcome.IsActive = updateOutcome.Outcome.IsActive;
                editOutcome.CriteriaPassRate = updateOutcome.Outcome.Criteria.PassRate;
                editOutcome.CalculateCriteriaPassRate = updateOutcome.Outcome.CalculateCriteriaPassRate;
                editOutcome.ModifiedDateTime = DateTime.Now;
                editOutcome.ModifiedByLoginID = UserData.PersonId;

                db.SaveChanges();

                return RedirectToAction("ViewOutcome", new RouteValueDictionary(new { controller = "Rubric", action = "ViewOutcome", outcomeID }));

            }
            catch
            {
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }
        */

        [Authorize(Roles = "EditOutcomesCriteria")]
        public ActionResult SaveOutcome(OutcomeViewModel outcomeViewModel)
        {
            try
            {
                Outcome outcome = db.Outcomes.Where(o => o.OutcomeID == outcomeViewModel.OutcomeVM.OutcomeID).FirstOrDefault();

                outcome.Name = outcomeViewModel.OutcomeVM.Name;
                outcome.Description = outcomeViewModel.OutcomeVM.Description;
                //check if the IsActive field changed status
                if (outcome.IsActive != outcomeViewModel.OutcomeVM.IsActive)
                {
                    //outcome is no longer active
                    if(!outcomeViewModel.OutcomeVM.IsActive)
                    {
                        outcome.InactiveDateTime = DateTime.Now;
                    }
                    else //outcome was inactive, now active
                    {
                        outcome.InactiveDateTime = null;
                    }
                }
                outcome.IsActive = outcomeViewModel.OutcomeVM.IsActive;
                outcome.CriteriaPassRate = outcomeViewModel.OutcomeVM.CriteriaPassRate / 100;
                outcome.CalculateCriteriaPassRate = outcomeViewModel.OutcomeVM.CalculateCriteriaPassRate;
                outcome.IsTSAOutcome = outcomeViewModel.OutcomeVM.IsTSAOutcome;
                outcome.ModifiedDateTime = DateTime.Now;
                outcome.ModifiedByLoginID = UserData.PersonId;

                db.SaveChanges();

                return RedirectToAction("ViewRubric", "Rubric", new { rubricID = outcomeViewModel.Rubric.RubricID, assessmentID = outcomeViewModel.Rubric.AssessmentID });
            }
            catch (Exception e)
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [Authorize(Roles = "EditOutcomesCriteria")]
        public Boolean MoveOutcome(int rubricID, int assessmentID, int outcomeID, int currentID)
        {
            //must be moving to a valid rubric
            var validRubric = db.RubricAssessments.Where(a => a.AssessmentID == assessmentID).Where(r => r.RubricID == rubricID).Count();

            //must get the object to update
            var outcome = db.Outcomes.Where(o => o.RubricID == currentID && o.OutcomeID == outcomeID).FirstOrDefault();

            //get the max sort order of outcomes for the new rubric and increment it
            int sortOrder = db.Outcomes.Where(o => o.RubricID == rubricID).Max(o => o.SortOrder);
            sortOrder++;

            if (outcome != null && validRubric > 0)
            {
                outcome.SortOrder = Convert.ToByte(sortOrder);
                outcome.RubricID = Convert.ToInt16(rubricID);
                outcome.ModifiedByLoginID = UserData.PersonId;
                outcome.ModifiedDateTime = DateTime.Now;
                db.SaveChanges();
                return true;
            }
            else return false;
        }

        [Authorize(Roles = "AddOutcomesCriteria")]
        public ActionResult AddCriterion(short outcomeID, short assessmentID)
        {
            Criterion criterion = new Criterion { OutcomeID = outcomeID };
            Outcome outcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();
            RubricAssessment rubric = db.RubricAssessments.Where(r => r.RubricID == outcome.RubricID && r.AssessmentID == assessmentID).FirstOrDefault();
            var model = new CriterionViewModel() { Criterion = criterion, Outcome = outcome, Rubric = rubric };
            model.Criterion.IsActive = true;
            return View(model);
        }

        [Authorize(Roles = "AddOutcomesCriteria")]
        public ActionResult InsertNewCriterion(CriterionViewModel criterionViewModel)
        {
            try
            {
                Criterion criterion = criterionViewModel.Criterion;
                Outcome outcome = db.Outcomes.Where(o => o.OutcomeID == criterion.OutcomeID).FirstOrDefault();
                Byte maxSortOrder = 0;

                if (db.Criteria.Where(c => c.OutcomeID == outcome.OutcomeID).Any())
                {
                    maxSortOrder = db.Criteria.Where(c => c.OutcomeID == outcome.OutcomeID).OrderByDescending(r => r.SortOrder).FirstOrDefault().SortOrder;
                }
                maxSortOrder++;

                criterion.SortOrder = maxSortOrder;
                criterion.CreatedDateTime = DateTime.Now;
                criterion.CreatedByLoginID = UserData.PersonId;
                outcome.Criteria.Add(criterion);
                db.Criteria.Add(criterion);

                db.SaveChanges();

                return RedirectToAction("ViewRubric", "Rubric", new { rubricID = criterionViewModel.Rubric.RubricID, assessmentID = criterionViewModel.Rubric.AssessmentID });
                // return RedirectToAction("ViewCriterion", new RouteValueDictionary(new { controller = "Rubric", action = "ViewCriterion", criterionID = addCriteria.CriteriaID }));

            }
            catch
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        [Authorize(Roles = "EditOutcomesCriteria")]
        public ActionResult EditCriterion(short criterionID, short assessmentID)
        {
            Criterion criterion = db.Criteria.Where(c => c.CriteriaID == criterionID).FirstOrDefault();
            RubricAssessment rubric = db.RubricAssessments.Where(r => r.RubricID == criterion.Outcome.RubricID && r.AssessmentID == assessmentID).FirstOrDefault();
            var model = new CriterionViewModel() { Criterion = criterion, Rubric = rubric };

            return View(model);
        }

        [Authorize(Roles = "EditOutcomesCriteria")]
        public ActionResult SaveCriterion(CriterionViewModel criterionViewModel)
        {
            try
            {
                Criterion criterion = db.Criteria.Where(c => c.CriteriaID == criterionViewModel.Criterion.CriteriaID).FirstOrDefault();

                //db.Criteria.Load();
                criterion.Name = criterionViewModel.Criterion.Name;
                criterion.ExampleText = criterionViewModel.Criterion.ExampleText;
                //if the active status has changed...
                if (criterion.IsActive !=  criterionViewModel.Criterion.IsActive)
                {
                    //criterion is no longer active, set the inactive date time
                    if (!criterionViewModel.Criterion.IsActive)
                    {
                        criterion.InactiveDateTime = DateTime.Now;
                    }
                    else // was inactive, now active
                    {
                        criterion.InactiveDateTime = null;
                    }
                }
                criterion.IsActive = criterionViewModel.Criterion.IsActive;
                criterion.ModifiedDateTime = DateTime.Now;
                criterion.ModifiedByLoginID = UserData.PersonId;

                //db.Entry(editCriteria).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ViewRubric", "Rubric", new { rubricID = criterionViewModel.Rubric.RubricID, assessmentID = criterionViewModel.Rubric.AssessmentID });
            }
            catch
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }
    }
}