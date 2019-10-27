using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CLS_SLE.Models;
using CLS_SLE.ViewModels;

namespace CLS_SLE.Controllers
{
    [Authorize(Roles ="Administrator")]

    public class RubricController : Controller
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        // GET: Rubric


        public ActionResult Index()
        {
            dynamic Model = new ExpandoObject();

            List<AssessmentRubric> Rubrics = (from Rubric in db.AssessmentRubrics
                                              select Rubric).ToList();
            Model.Rubrics = Rubrics;
            return View(Model);
        }


        public ActionResult ViewRubric(int? rubricID)
        {
            dynamic Model = new ExpandoObject();
            var Rubric = db.AssessmentRubrics.Where(r => r.RubricID == rubricID).FirstOrDefault();
            Model.Outcomes = db.Outcomes.Where(o => o.RubricID == rubricID).ToList();
            Model.Rubric = Rubric;
            Model.CreatorLogin = null;
            Model.ModifierLogin = null;

            if (Rubric.CreatedByLoginID != null)
            {
                Model.CreatorLogin = (String)db.Users.Where(u => u.PersonID == Rubric.CreatedByLoginID).FirstOrDefault().Login;
            }
            if (Rubric.ModifiedByLoginID != null)
            {
                Model.ModifierLogin = (String)db.Users.Where(u => u.PersonID == Rubric.ModifiedByLoginID).FirstOrDefault().Login;
            }
            return View(Model);
        }
        [HttpGet]
        public ActionResult AddRubric(short assessmentID)
        {
            
           RubricAssessment rubricAssessment = db.RubricAssessments.Where(r => r.AssessmentID == assessmentID).FirstOrDefault();
            rubricAssessment.AssessmentID = assessmentID;
            //assessmentID = Convert.ToInt16(rubricAssessment.AssessmentID);
            
            return View();
        }

        [HttpPost]
        public ActionResult InsertNewRubric(RubricViewModel rubricViewModel)
        {
            try
            {
                //KEEP
                db.AssessmentRubrics.Load();
                db.RubricAssessments.Load();
                short assessmentID = Convert.ToInt16(rubricViewModel.AssessmentID);
                rubricViewModel.RubricAssesssment.AssessmentID = assessmentID;
                rubricViewModel.AssessmentRubric.CreatedDateTime = DateTime.Now;
                rubricViewModel.AssessmentRubric.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                rubricViewModel.RubricAssesssment.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                rubricViewModel.RubricAssesssment.CreatedDateTime = DateTime.Now;
                db.RubricAssessments.Add(rubricViewModel.RubricAssesssment);
                db.AssessmentRubrics.Add(rubricViewModel.AssessmentRubric);

                db.SaveChanges();

                return RedirectToAction("ViewRubric", new RouteValueDictionary(new { controller = "Rubric", action = "ViewRubric", rubricViewModel.RubricAssesssment.RubricID}));

            }
            catch(Exception e)
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }


        public ActionResult EditRubric(short rubricID)
        {
           RubricAssessment rubricAssessment = db.RubricAssessments.Where(r => r.RubricID == rubricID).FirstOrDefault();
            AssessmentRubric assessmentRubric = db.AssessmentRubrics.Where(a => a.RubricID == rubricID).FirstOrDefault();

            ViewBag.Id = rubricAssessment.RubricID;
            ViewBag.Name = assessmentRubric.Name;
            ViewBag.Description = assessmentRubric.Description;
            ViewBag.StartDate = (rubricAssessment.StartDate).ToString("MM/dd/yyyy");
            ViewBag.EndDate = (rubricAssessment.EndDate);

            return View();
        }

        public ActionResult SaveRubric(UpdateRubric updateRubric, short rubricID)
        {
            try
            {
                //db.AssessmentRubrics.Load();
                //db.RubricAssessments.Load();
                AssessmentRubric editRubric = db.AssessmentRubrics.Where(r => r.RubricID == rubricID).FirstOrDefault();
                RubricAssessment rubricAssessment = db.RubricAssessments.Where(r => r.RubricID == rubricID).FirstOrDefault();

                editRubric.Name = ViewBag.Name;
                editRubric.Description = ViewBag.Description;
                editRubric.IsActive = ViewBag.IsActive;
                editRubric.ModifiedDateTime = DateTime.Now;
                editRubric.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                //db.Entry(editRubric).State = EntityState.Modified;
                db.SaveChanges();

                //if (!editRubric.IsActive)
                //{
                //    db.Criteria.Load();
                //    db.Outcomes.Load();
                //    var Outcomes = db.Outcomes.Where(o => o.RubricID == rubricID);
                //    var Criteria = db.Criteria.Where(c => c.OutcomeID.Equa )
                //}
                //return RedirectToAction(actionName: "Rubric", controllerName: "Rubric", routeValues: "rubricID" = rubricID);
                return RedirectToAction("ViewRubric", new RouteValueDictionary(new { controller = "Rubric", action = "ViewRubric", rubricID }));
            }
            catch(Exception e)
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }


        public ActionResult ViewOutcome(int? outcomeID)
        {
            dynamic Model = new ExpandoObject();
            var Outcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();
            var Criteria = db.Criteria.Where(c => c.OutcomeID == outcomeID).ToList();
            Model.Outcome = Outcome;
            Model.Criteria = Criteria;
            Model.CreatorLogin = null;
            Model.ModifierLogin = null;

            //if (Outcome.CreatedByLoginID != null)
            //{
            //    Model.CreatorLogin = (String)db.Users.Where(u => u.PersonID == Outcome.CreatedByLoginID).FirstOrDefault().Login;
            //}
            //if (Outcome.ModifiedByLoginID != null)
            //{
            //    Model.ModifierLogin = (String)db.Users.Where(u => u.PersonID == Outcome.ModifiedByLoginID).FirstOrDefault().Login;
            //}
            return View(Model);
        }
        public ActionResult AddOutcome(int? rubricID)
        {
            dynamic Model = new ExpandoObject();
            Model.Rubric = db.AssessmentRubrics.Where(r => r.RubricID == rubricID).FirstOrDefault();
            return View(Model);
        }
        public ActionResult InsertNewOutcome(FormCollection formCollection)
        {
            try
            {
                Int16 RubricId = Int16.Parse(formCollection["RubricId"]);
                //Byte SortOrder = Byte.Parse(formCollection["SortOrder"]);\

                db.Outcomes.Load();
                Outcome addOutcome = db.Outcomes.Create();

                addOutcome.RubricID = RubricId; 
                addOutcome.Name = formCollection["Name"];
                addOutcome.Description = formCollection["Description"];
                addOutcome.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                //addOutcome.SortOrder = SortOrder;
                addOutcome.SortOrder = 1;
                addOutcome.CriteriaPassRate = (Decimal?)(Double.Parse(formCollection["PassPercent"])) / 100;
                addOutcome.CalculateCriteriaPassRate = ((formCollection["CalculateCriteriaPassRate"]).Equals("True") ? true : false);
                addOutcome.CreatedDateTime = DateTime.Now;
                addOutcome.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                db.Entry(addOutcome).State = EntityState.Added;
                db.SaveChanges();

                return RedirectToAction("ViewOutcome", new RouteValueDictionary(new { controller = "Rubric", action = "ViewOutcome", addOutcome.OutcomeID }));

            }
            catch
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }
        public ActionResult EditOutcome(int? outcomeID, int? rubricID)
        {
            dynamic Model = new ExpandoObject();
            Model.Outcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();
            return View(Model);
        }

        public ActionResult SaveOutcome(int? outcomeID, FormCollection formCollection)
        {
            try
            {
                Int16 RubricId = Int16.Parse(formCollection["RubricId"]);
                //Byte SortOrder = Byte.Parse(formCollection["SortOrder"]);

                db.Outcomes.Load();
                Outcome editOutcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();

                editOutcome.RubricID = RubricId;
                editOutcome.Name = formCollection["Name"];
                editOutcome.Description = formCollection["Description"];
                editOutcome.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                //editOutcome.SortOrder = SortOrder;
                editOutcome.CriteriaPassRate = (Decimal?)(Double.Parse(formCollection["PassPercent"])) / 100;
                editOutcome.CalculateCriteriaPassRate = ((formCollection["CalculateCriteriaPassRate"]).Equals("True") ? true : false);
                editOutcome.ModifiedDateTime = DateTime.Now;
                editOutcome.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                db.Entry(editOutcome).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ViewOutcome", new RouteValueDictionary(new { controller = "Rubric", action = "ViewOutcome", outcomeID }));

            }
            catch
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        public ActionResult ViewCriterion(int? criterionID)
        {
            dynamic Model = new ExpandoObject();
            var Criterion = db.Criteria.Where(c => c.CriteriaID == criterionID).FirstOrDefault();
            Model.Criterion = Criterion;
            Model.CreatorLogin = null;
            Model.ModifierLogin = null;

            if (Criterion.CreatedByLoginID != null)
            {
                Model.CreatorLogin = (String)db.Users.Where(u => u.PersonID == Criterion.CreatedByLoginID).FirstOrDefault().Login;
            }
            if (Criterion.ModifiedByLoginID != null)
            {
                Model.ModifierLogin = (String)db.Users.Where(u => u.PersonID == Criterion.ModifiedByLoginID).FirstOrDefault().Login;
            }
            return View(Model);
        }
        public ActionResult AddCriterion(int? outcomeID)
        {
            dynamic Model = new ExpandoObject();
            Model.Outcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();
            return View(Model);
        }

        public ActionResult InsertNewCriterion(FormCollection formCollection)
        {
            try
            {
                Int16 OutcomeID = Int16.Parse(formCollection["OutcomeID"]);
                Byte maxSortOrder = 0;
                if (db.Criteria.Where(c => c.OutcomeID == OutcomeID).Any())
                {
                    maxSortOrder = db.Criteria.Where(c => c.OutcomeID == OutcomeID).OrderByDescending(r => r.SortOrder).FirstOrDefault().SortOrder;
                }
                maxSortOrder++;

                db.Criteria.Load();
                Criterion addCriteria = db.Criteria.Create();

                addCriteria.OutcomeID = OutcomeID;
                addCriteria.Name = formCollection["Name"];
                addCriteria.ExampleText = formCollection["ExampleText"];
                addCriteria.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                addCriteria.SortOrder = maxSortOrder;
                addCriteria.CreatedDateTime = DateTime.Now;
                addCriteria.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                db.Entry(addCriteria).State = EntityState.Added;
                db.SaveChanges();

                return RedirectToAction("ViewCriterion", new RouteValueDictionary(new { controller = "Rubric", action = "ViewCriterion", criterionID = addCriteria.CriteriaID }));

            }
            catch
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }
        public ActionResult EditCriterion(int? criterionID)
        {
            dynamic Model = new ExpandoObject();
            Model.Criterion = db.Criteria.Where(c => c.CriteriaID == criterionID).FirstOrDefault();

            return View(Model);
        }
        public ActionResult SaveCriterion(int? criterionID, FormCollection formCollection)
        {
            try
            {
                Int16 CriteriaId = Int16.Parse(formCollection["CriteriaId"]);              

                db.Criteria.Load();
                Criterion editCriteria = db.Criteria.Where(c => c.CriteriaID == criterionID).FirstOrDefault();
                editCriteria.Name = formCollection["Name"];
                editCriteria.ExampleText = formCollection["ExampleText"];
                editCriteria.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                
                editCriteria.ModifiedDateTime = DateTime.Now;
                editCriteria.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                db.Entry(editCriteria).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ViewCriterion", new RouteValueDictionary(new { controller = "Rubric", action = "ViewCriterion", criterionID = editCriteria.CriteriaID }));

            }
            catch
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }
    }
}