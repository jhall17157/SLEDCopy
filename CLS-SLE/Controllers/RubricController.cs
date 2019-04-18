using System;
using System.Collections.Generic;
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
            return View();
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
        public ActionResult AddOutcome(int? rubricID)
        {
            return View();
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