using CLS_SLE.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using CLS_SLE.ViewModels;

namespace CLS_SLE.Controllers
{
    public class AdminAssessmentsController : Controller
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();
        private Logger logger = LogManager.GetCurrentClassLogger();

        // GET: AdminAssessments
        public ActionResult Index()
        {
            return View();
        }
        private ActionResult Exceptions()
        {
            return RedirectToAction(actionName: "AdminDashboard", controllerName: "Admin");
        }

        public ActionResult AssessmentMappings()
        {
            return View();
        }

        public ActionResult Assessments()
        {
            try
            {
                var personID = Convert.ToInt32(Session["personID"].ToString());
                var user = db.Users.FirstOrDefault(u => u.PersonID == personID);
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
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

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

                Model.Assessment = assessment;
                return View(Model);
            }
            catch
            {
                logger.Error("User attempted to load dashboard without being signed in, redirecting to sign in page.");
                return Exceptions();
            }
        }

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

        //no view was associated with this method below
        [HttpPost]
        public ActionResult InsertNewAssessment(
            InsertNewAssesmentViewModel insertNewAssesmentViewModel,
            string category)
        {
            try
            {
                db.Assessments.Load();
                //string Category = formCollection["Category"];
                var CategoryCode = db.AssessmentCategories.Where(c => c.Name == category).FirstOrDefault().CategoryCode;
                var program = insertNewAssesmentViewModel.Assessment.Program;
                Assessment addAssessment = db.Assessments.Create();


                addAssessment.Name = insertNewAssesmentViewModel.Assessment.Name;
                addAssessment.Category = CategoryCode;
                addAssessment.Description = insertNewAssesmentViewModel.Assessment.Description != null ? 
                    insertNewAssesmentViewModel.Assessment.Description : "";
                addAssessment.OutcomePassRate = (Decimal?)(Double.Parse(Regex.Replace(insertNewAssesmentViewModel.Assessment.OutcomePassRate, "[^0-9.]", ""));
			    addAssessment.CalculateOutcomePassRate = ((formCollection["CalculateOutcomePassRate"]).Equals("True") ? true : false);
                addAssessment.ProgramID = db.Programs.Where(p => p.Name == program).FirstOrDefault().ProgramID;
                addAssessment.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                addAssessment.CreatedDateTime = DateTime.Now;
                addAssessment.CreatedByLoginID = Convert.ToInt32(Session["personID"].ToString());



                db.Entry(addAssessment).State = EntityState.Added;
                db.SaveChanges();


                return RedirectToAction(actionName: "Assessments", controllerName: "AdminAssessments");

            }
            catch (Exception e)
            {
                logger.Error("Failed to save assessment, redirecting to sign in page.");
                logger.Error(e.Message);
                logger.Error(e.StackTrace);
                return RedirectToAction(actionName: "Signin", controllerName: "User");
            }
        }

        //no method was associated with this method below
        [HttpPost]
        public ActionResult SaveAssessment(FormCollection formCollection)
        {
            try
            {
                if (Int32.Parse(formCollection["AssessmentID"]) > 0)
                {
                    var assessmentid = Int32.Parse(formCollection["AssessmentID"]);
                    var editAssessment = db.Assessments.FirstOrDefault(a => a.AssessmentID == assessmentid);

                    if (editAssessment != null)
                    {
                        editAssessment.Name = formCollection["Name"];
                        editAssessment.Category = formCollection["Category"];
                        editAssessment.Description = formCollection["Description"];
                        editAssessment.OutcomePassRate = (Decimal?)(Double.Parse(formCollection["PassPercent"])) / 100;
                        editAssessment.CalculateOutcomePassRate = ((formCollection["CalculateOutcomePassRate"]).Equals("True") ? true : false);
                        var program = (formCollection["Program"]);
                        editAssessment.ProgramID = db.Programs.Where(p => p.Name == program).FirstOrDefault().ProgramID;
                        editAssessment.IsActive = ((formCollection["IsActive"]).Equals("True") ? true : false);
                        editAssessment.ModifiedDateTime = DateTime.Now;
                        editAssessment.ModifiedByLoginID = Convert.ToInt32(Session["personID"].ToString());
                        db.SaveChanges();

                        return RedirectToAction(actionName: "ViewAssessment", controllerName: "AdminAssessments", routeValues: new { assessmentId = editAssessment.AssessmentID });
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
            catch
            {
                logger.Error("Failed to save assessment, redirecting to sign in page.");
                return RedirectToAction(actionName: "Signin", controllerName: "User");
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
    }
}