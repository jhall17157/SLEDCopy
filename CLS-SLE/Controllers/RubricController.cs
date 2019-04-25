using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.Models;

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
            Model.Rubric = db.AssessmentRubrics.Where(r => r.RubricID == rubricID).FirstOrDefault();
            Model.Outcomes = db.Outcomes.Where(o => o.RubricID == rubricID).ToList();
            return View(Model);
        }
        public ActionResult AddRubric(int? assessmentID)
        {
            dynamic model = new ExpandoObject();
            model.assessmentID = null;

            if (assessmentID != null)
            {
                model.assessmentID = assessmentID;
            }
            return View(model);
        }
        public ActionResult InsertNewRubric(FormCollection formCollection)
        {
            try
            {
                db.AssessmentRubrics.Load();
                db.RubricAssessments.Load();
                AssessmentRubric addRubric = db.AssessmentRubrics.Create();
                if (formCollection["assessmentID"].Equals("0") || formCollection["assessmentID"] == null)
                {
                    addRubric.AssessmentID = null;
                } else
                {
                    addRubric.AssessmentID = Int16.Parse(formCollection["assessmentID"]);
                }
                addRubric.Name = formCollection["Name"];
                addRubric.Description = formCollection["Description"];
                addRubric.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                addRubric.CreatedDateTime = DateTime.Now;
                addRubric.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                db.Entry(addRubric).State = EntityState.Added;
                db.SaveChanges();

                RubricAssessment rubricAssessment = db.RubricAssessments.Create();
                rubricAssessment.AssessmentID = (Int16)addRubric.AssessmentID;
                rubricAssessment.RubricID = addRubric.RubricID;
                rubricAssessment.CreatedDateTime = addRubric.CreatedDateTime;
                rubricAssessment.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                rubricAssessment.StartDate = DateTime.Parse(formCollection["startDate"]);
                rubricAssessment.EndDate = DateTime.Parse(formCollection["endDate"]);
                db.Entry(rubricAssessment).State = EntityState.Added;
                db.SaveChanges();

                return RedirectToAction(actionName: "Index", controllerName: "Rubric");

            }
            catch
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }
        public ActionResult EditRubric(int? rubricID)
        {
            return View();
        }


        public ActionResult ViewOutcome(int? outcomeID)
        {
            dynamic Model = new ExpandoObject();
            Model.Outcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();
            Model.Criteria = db.Criteria.Where(c => c.OutcomeID == outcomeID).ToList();
            return View(Model);
        }
        public ActionResult AddOutcome(int? id)
        {
            dynamic Model = new ExpandoObject();
            Model.ID = id;
            return View(Model);
        }
        public ActionResult InsertNewOutcome(FormCollection formCollection)
        {
            try
            {
                Int16 RubricId = Int16.Parse(formCollection["RubricId"]);
                Byte SortOrder = Byte.Parse(formCollection["SortOrder"]);

                db.Outcomes.Load();
                Outcome addOutcome = db.Outcomes.Create();

                addOutcome.RubricID = RubricId; //default to 1 until we know how to handle properly
                addOutcome.Name = formCollection["Name"];
                addOutcome.Description = formCollection["Description"];
                addOutcome.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                addOutcome.SortOrder = SortOrder;
                addOutcome.CriteriaPassRate = (Decimal?)(Double.Parse(formCollection["PassPercent"])) / 100;
                addOutcome.CalculateCriteriaPassRate = ((formCollection["CalculateCriteriaPassRate"]).Equals("True") ? true : false);
                addOutcome.CreatedDateTime = DateTime.Now;
                addOutcome.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                db.Entry(addOutcome).State = EntityState.Added;
                db.SaveChanges();

                return RedirectToAction(actionName: "Index", controllerName: "Rubric");

            }
            catch
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }
        public ActionResult EditOutcome(int? outcomeID)
        {
            return View();
        }


        public ActionResult ViewCriterion(int? criterionID)
        {
            dynamic Model = new ExpandoObject();
            Model.Criterion = db.Criteria.Where(c => c.CriteriaID == criterionID).FirstOrDefault();
            return View(Model);
        }
        public ActionResult AddCriterion(int? outcomeID)
        {
            return View();
        }
        public ActionResult EditCriterion(int? criterionID)
        {
            return View();
        }
    }
}