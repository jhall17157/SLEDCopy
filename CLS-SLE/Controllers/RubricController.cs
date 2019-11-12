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
	[Authorize(Roles = "Administrator")]

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

            //if (Rubric.CreatedByLoginID != null)
            //{
            //	Model.CreatorLogin = (String)db.Users.Where(u => u.PersonID == Rubric.CreatedByLoginID).FirstOrDefault().Login;
            //}
            //if (Rubric.ModifiedByLoginID != null)
            //{
            //	Model.ModifierLogin = (String)db.Users.Where(u => u.PersonID == Rubric.ModifiedByLoginID).FirstOrDefault().Login;
            //}
            return View(Model);
        }
        [HttpGet]
		public ActionResult AddRubric(short? assessmentID)
		{
			RubricAssessment rubricAssessment = db.RubricAssessments.Where(r => r.AssessmentID == assessmentID).FirstOrDefault();
			// rubricAssessment.AssessmentID = assessmentID;
			//assessmentID = Convert.ToInt16(rubricAssessment.AssessmentID);

			ViewBag.Assessments = db.Assessments.Select(a => a.Name).ToList();
			ViewBag.InitialAssessment = db.Assessments.Where(a => a.AssessmentID == assessmentID).FirstOrDefault().Name;
			ViewBag.AssessmentID = assessmentID;

			return View();
		}

		//post for submitting a new Rubric
		[HttpPost]
		public ActionResult InsertNewRubric(RubricViewModel rubricViewModel, FormCollection formCollection)
		{
			try
			{
				// RubricAssessment rubricAssessment = rubricViewModel.RubricAssesssment;
				// AssessmentRubric assessmentRubric = rubricViewModel.AssessmentRubric;
				int personID = Convert.ToInt32(Session["personID"].ToString());
				List<Assessment> assessments = new List<Assessment>();

				foreach (var key in formCollection.AllKeys)
					if (key.Contains("relatedAssessment"))
					{
						string name = formCollection[key];
						assessments.Add(db.Assessments.Where(a => a.Name == name).FirstOrDefault());
					}

				db.AssessmentRubrics.Load();

				rubricViewModel.AssessmentRubric.CreatedDateTime = DateTime.Now;
				rubricViewModel.AssessmentRubric.CreatedByLoginID = personID;

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
					rubricAssessment.CreatedByLoginID = personID;

					db.RubricAssessments.Add(rubricAssessment);
				}

				db.SaveChanges();

				return RedirectToAction("ViewRubric", "Rubric", new { rubricID = rubricViewModel.AssessmentRubric.RubricID });

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

				return RedirectToAction("ViewRubric", new RouteValueDictionary(new { controller = "Rubric", action = "ViewRubric", rubricViewModel.RubricAssesssment.RubricID }));

			}
			catch (Exception e)
			{
				//logger.Error("Failed to save assessment, redirecting to sign in page.");
				return Content("<html>Message:<br>" + e.Message + "<br>Inner Exception:<br>" + e.InnerException + "<br>Stack Trace:<br>" + e.StackTrace + "</html>");
				return RedirectToAction(actionName: "Signin", controllerName: "User");
			}
		}

		[HttpGet]
		public ActionResult EditRubric(short rubricID)
		{
			RubricAssessment rubricAssessment = db.RubricAssessments.Where(r => r.RubricID == rubricID).FirstOrDefault();
			AssessmentRubric assessmentRubric = db.AssessmentRubrics.Where(a => a.RubricID == rubricID).FirstOrDefault();
			List<short> relatedAssessmentIDs = db.RubricAssessments.Where(r => r.RubricID == rubricID).Select(r => r.AssessmentID).ToList();
			List<string> relatedAssessments = new List<string>();

			foreach (short relatedAssessmentID in relatedAssessmentIDs)
				relatedAssessments.Add(db.Assessments.Where(a => a.AssessmentID == relatedAssessmentID).FirstOrDefault().Name);

			ViewBag.Id = assessmentRubric.RubricID;
			ViewBag.Name = assessmentRubric.Name;
			ViewBag.Description = assessmentRubric.Description;
			ViewBag.StartDate = (rubricAssessment.StartDate).ToString("MM/dd/yyyy");
			ViewBag.EndDate = (rubricAssessment.EndDate);
			ViewBag.IsActive = assessmentRubric.IsActive;
			ViewBag.RelatedAssessments = relatedAssessments;
			ViewBag.Assessments = db.Assessments.Select(a => a.Name).ToList();

			// rubricAssessment.ModifiedDateTime = DateTime.Now;
			// rubricAssessment.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());
			// assessmentRubric.ModifiedDateTime = DateTime.Now;
			// assessmentRubric.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());

			return View();
		}

		//Post for saving an edited rubric
		[HttpPost]
		public ActionResult SaveRubric(UpdateRubric updateRubric, short rubricID, FormCollection formCollection)
		{
			try
			{
				AssessmentRubric editRubric = db.AssessmentRubrics.Where(r => r.RubricID == rubricID).FirstOrDefault();
				// RubricAssessment rubricAssessment = db.RubricAssessments.Where(r => r.RubricID == rubricID).FirstOrDefault();
				int personID = Convert.ToInt32(Session["personID"].ToString());
				List<string> assessments = new List<string>();
				List<short> relatedAssessmentIDs = db.RubricAssessments.Where(r => r.RubricID == rubricID).Select(r => r.AssessmentID).ToList();
				List<string> relatedAssessments = new List<string>();

				foreach (var key in formCollection.AllKeys)
					if (key.Contains("relatedAssessment"))
					{
						string name = formCollection[key];
						assessments.Add(name);
					}

				foreach (short relatedAssessmentID in relatedAssessmentIDs)
					relatedAssessments.Add(db.Assessments.Where(a => a.AssessmentID == relatedAssessmentID).FirstOrDefault().Name);

				db.RubricAssessments.Load();

				foreach (string assessment in relatedAssessments)
					if (!assessments.Contains(assessment))
					{
						short assessmentID = db.Assessments.Where(a => a.Name == assessment).FirstOrDefault().AssessmentID;

						db.RubricAssessments.Remove(db.RubricAssessments.Where(r => r.AssessmentID == assessmentID && r.RubricID == rubricID).FirstOrDefault());
					}

				foreach (string assessment in assessments)
				{
					short assessmentID = db.Assessments.Where(a => a.Name == assessment).FirstOrDefault().AssessmentID;

					if (db.RubricAssessments.Where(r => r.AssessmentID == assessmentID && r.RubricID == rubricID).Any())
					{
						RubricAssessment rubricAssessment = db.RubricAssessments.Where(r => r.AssessmentID == assessmentID && r.RubricID == rubricID).FirstOrDefault();

						rubricAssessment.StartDate = updateRubric.RubricAssessment.StartDate;
						rubricAssessment.EndDate = updateRubric.RubricAssessment.EndDate;
						rubricAssessment.ModifiedDateTime = DateTime.Now;
						rubricAssessment.ModifiedByLoginID = personID;
					}
					else
					{
						RubricAssessment rubricAssessment = new RubricAssessment();

						rubricAssessment.AssessmentID = assessmentID;
						rubricAssessment.RubricID = rubricID;
						rubricAssessment.StartDate = updateRubric.RubricAssessment.StartDate;
						rubricAssessment.EndDate = updateRubric.RubricAssessment.EndDate;
						rubricAssessment.CreatedDateTime = DateTime.Now;
						rubricAssessment.CreatedByLoginID = personID;

						db.RubricAssessments.Add(rubricAssessment);
					}
				}

				editRubric.Name = updateRubric.AssessmentRubric.Name;
				editRubric.Description = updateRubric.AssessmentRubric.Description;
				editRubric.IsActive = updateRubric.AssessmentRubric.IsActive;
				editRubric.ModifiedDateTime = DateTime.Now;
				editRubric.ModifiedByLoginID = personID;

				db.SaveChanges();

				return RedirectToAction("ViewRubric", new RouteValueDictionary(new { controller = "Rubric", action = "ViewRubric", rubricID }));
			}
			catch (Exception e)
			{
				//logger.Error("Failed to save assessment, redirecting to sign in page.");
				return Content("<html>Message:<br>" + e.Message + "<br>Inner Exception:<br>" + e.InnerException + "<br>Stack Trace:<br>" + e.StackTrace + "</html>");
				return RedirectToAction(actionName: "Signin", controllerName: "User");
			}
		}

		[HttpGet]
        public ActionResult AddOutcome(short rubricID)
       {
           AssessmentRubric rubric = db.AssessmentRubrics.Where(r => r.RubricID == rubricID).FirstOrDefault();
           rubric.RubricID = rubricID;
           return View();
       }

       [HttpPost]
       public ActionResult InsertNewOutcome(OutcomeViewModel outcomeViewModel)
       {
            try
            {
                short rubricID = Convert.ToInt16(outcomeViewModel.RubricID);
                Byte maxSortOrder = 0;
                if (db.Outcomes.Where(c => c.RubricID == rubricID).Any())
                {
                    maxSortOrder = db.Outcomes.Where(c => c.RubricID == rubricID).OrderByDescending(r => r.SortOrder).FirstOrDefault().SortOrder;
                }
                maxSortOrder++;
                
                short outcomeID = Convert.ToInt16(outcomeViewModel.OutcomeVM.OutcomeID);
                db.Outcomes.Load();
                AssessmentRubric rubric = db.AssessmentRubrics.Where(r => r.RubricID == rubricID).FirstOrDefault();
                outcomeViewModel.OutcomeVM.SortOrder = maxSortOrder;
                outcomeViewModel.OutcomeVM.CreatedDateTime = DateTime.Now;
                outcomeViewModel.OutcomeVM.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                rubric.Outcomes.Add(outcomeViewModel.OutcomeVM);
                db.Outcomes.Add(outcomeViewModel.OutcomeVM);
                db.SaveChanges();

                return RedirectToAction("ViewRubric", new RouteValueDictionary(new { controller = "Rubric", action = "ViewRubric", rubricID }));
            }
            catch (Exception e)
            {
                //logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
       }


        [HttpGet]
        public ActionResult EditOutcome(short outcomeID)
        {
            db.Outcomes.Load();
            //AssessmentRubric assessmentRubric = db.AssessmentRubrics.Where(a => a.RubricID == rubricID).FirstOrDefault();
            Outcome outcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();

            ViewBag.OutcomeId = outcome.OutcomeID;
            ViewBag.RubricId = outcome.RubricID;
            ViewBag.Name = outcome.Name;
            ViewBag.Description = outcome.Description;
            ViewBag.CriteriaPassRate = outcome.CriteriaPassRate;
            ViewBag.IsActive = outcome.IsActive;
            ViewBag.CalculateCriteriaPassRate = outcome.CalculateCriteriaPassRate;
            ViewBag.CriteriaPassRate = outcome.CriteriaPassRate * 100;

            return View();
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
                editOutcome.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());

                db.SaveChanges();

                return RedirectToAction("ViewOutcome", new RouteValueDictionary(new { controller = "Rubric", action = "ViewOutcome", outcomeID }));

            }
            catch
            {
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }
        */

        public ActionResult SaveOutcome(FormCollection formCollection, short outcomeID, short rubricID)
		{
			try
			{
				// Int16 RubricId = Int16.Parse(formCollection["RubricId"]);
				// Byte SortOrder = Byte.Parse(formCollection["SortOrder"]);

				db.Outcomes.Load();
				Outcome editOutcome = db.Outcomes.Where(o => o.OutcomeID == outcomeID).FirstOrDefault();

				editOutcome.RubricID = rubricID;
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

				return RedirectToAction("ViewRubric", "Rubric", new { rubricID });
				// return RedirectToAction("ViewOutcome", new RouteValueDictionary(new { controller = "Rubric", action = "ViewOutcome", outcomeID }));

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

		public ActionResult InsertNewCriterion(FormCollection formCollection, short outcomeID, short rubricID)
		{
			try
			{
				// Int16 OutcomeID = Int16.Parse(formCollection["OutcomeID"]);
				Byte maxSortOrder = 0;
				if (db.Criteria.Where(c => c.OutcomeID == outcomeID).Any())
				{
					maxSortOrder = db.Criteria.Where(c => c.OutcomeID == outcomeID).OrderByDescending(r => r.SortOrder).FirstOrDefault().SortOrder;
				}
				maxSortOrder++;

				db.Criteria.Load();
				Criterion addCriteria = db.Criteria.Create();

				addCriteria.OutcomeID = outcomeID;
				addCriteria.Name = formCollection["Name"];
				addCriteria.ExampleText = formCollection["ExampleText"];
				addCriteria.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
				addCriteria.SortOrder = maxSortOrder;
				addCriteria.CreatedDateTime = DateTime.Now;
				addCriteria.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());

				db.Entry(addCriteria).State = EntityState.Added;
				db.SaveChanges();

				return RedirectToAction("ViewRubric", "Rubric", new { rubricID });
				// return RedirectToAction("ViewCriterion", new RouteValueDictionary(new { controller = "Rubric", action = "ViewCriterion", criterionID = addCriteria.CriteriaID }));

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

			var Criterion = db.Criteria.Where(c => c.CriteriaID == criterionID).FirstOrDefault();
			var Outcome = db.Outcomes.Where(o => o.OutcomeID == Criterion.OutcomeID).FirstOrDefault();

			Model.Criterion = Criterion;
			Model.RubricID = Outcome.RubricID;

			return View(Model);
		}
		public ActionResult SaveCriterion(FormCollection formCollection, int? criterionID, int? rubricID)
		{
			try
			{
				// Int16 CriteriaId = Int16.Parse(formCollection["CriteriaId"]);

				db.Criteria.Load();
				Criterion editCriteria = db.Criteria.Where(c => c.CriteriaID == criterionID).FirstOrDefault();
				editCriteria.Name = formCollection["Name"];
				editCriteria.ExampleText = formCollection["ExampleText"];
				editCriteria.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);

				editCriteria.ModifiedDateTime = DateTime.Now;
				editCriteria.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());

				db.Entry(editCriteria).State = EntityState.Modified;
				db.SaveChanges();

				return RedirectToAction("ViewRubric", "Rubric", new { rubricID });
				// return RedirectToAction("ViewCriterion", new RouteValueDictionary(new { controller = "Rubric", action = "ViewCriterion", criterionID = editCriteria.CriteriaID }));

			}
			catch
			{
				//logger.Error("Failed to save assessment, redirecting to sign in page.");
				return RedirectToAction(actionName: "Signin", controllerName: "User");
			}
		}
	}
}