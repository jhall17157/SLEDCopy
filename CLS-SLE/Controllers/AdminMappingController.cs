using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{

    //New controller for mapping page
    public class AdminMappingController : SLEControllerBase
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();


        // GET: AdminMapping home page with list of Programs sorted by id
        public ActionResult Index()
        {
            MappingViewModel mappingViewModel = new MappingViewModel();
            mappingViewModel.Programs = (from p in db.Programs
                                         where p.Number != "000000" && p.Number != "999999"
                                         select new SelectListItem { Text = p.Number + " " + p.Name, Value = p.ProgramID.ToString() })
                                            .Distinct().ToList();
            mappingViewModel.Programs = mappingViewModel.Programs.OrderBy(p => p.Text).ToList();
            mappingViewModel.Courses = (from c in db.Courses
                                        where !c.CourseName.Contains("Folio180")
                                        select new SelectListItem { Text = c.Number + " " + c.CourseName, Value = c.CourseID.ToString() })
                                            .Distinct().ToList();
            mappingViewModel.Rubrics = (from r in db.AssessmentRubrics
                                        select new SelectListItem { Text = r.Name, Value = r.RubricID.ToString() }).Distinct().ToList();
            if (TempData["ProgramID"] != null)
            {
                int programID = (int)TempData["ProgramID"];


                mappingViewModel.Program = LoadVmProgram(programID);

                // mappingViewModel.Program = db.Programs.FirstOrDefault(p => p.ProgramID == programID);
                mappingViewModel.ProgramID = programID;
            }
            return View(mappingViewModel);
        }

        //returns index page with viewmodel populated by selected program
        [HttpPost]
        public ActionResult Index(MappingViewModel mappingVM)
        {
            MappingViewModel mappingViewModel = new MappingViewModel();
            mappingViewModel.Programs = (from p in db.Programs
                                         select new SelectListItem { Text = p.Number + " " + p.Name, Value = p.ProgramID.ToString() })
                                            .Distinct().ToList();
            mappingViewModel.Courses = (from c in db.Courses
                                        where !c.CourseName.Contains("Folio180")
                                        select new SelectListItem { Text = c.Number + " " + c.CourseName, Value = c.CourseID.ToString() })
                                            .Distinct().ToList();
            mappingViewModel.Rubrics = (from r in db.AssessmentRubrics
                                        where r.RubricAssessments.Where(ra => ra.Assessment.ProgramID == mappingVM.ProgramID).Any()
                                        select new SelectListItem { Text = r.Name, Value = r.RubricID.ToString() }).Distinct().ToList();

            mappingViewModel.Rubrics.AddRange((from r in db.AssessmentRubrics
                                               where r.RubricAssessments.Where(ra => ra.Assessment.ProgramID == 0).Any()
                                               select new SelectListItem { Text = r.Name, Value = r.RubricID.ToString() }).Distinct().ToList());

            mappingViewModel.Course = db.Courses.FirstOrDefault(p => p.CourseID == mappingVM.CourseID);
            int programID;
            if (TempData["ProgramID"] != null)
            {
                mappingViewModel.ProgramID = (int)TempData["ProgramID"];
            }
            else
            {
                mappingViewModel.ProgramID = mappingVM.ProgramID;
            }


            mappingViewModel.Program = LoadVmProgram(mappingVM.ProgramID);


            //mappingViewModel.Program = db.Programs.FirstOrDefault(p => p.ProgramID == mappingViewModel.ProgramID);

            return View(mappingViewModel);
        }


        //create new mapping
        [HttpPost]
        public ActionResult CreateMapping(MappingViewModel mappingVM)
        {
            TempData["ProgramID"] = mappingVM.ProgramID;
            if (ModelState.IsValid)
            {
                ProgramAssessmentMapping map = new ProgramAssessmentMapping();
                map.CourseID = Convert.ToInt16(mappingVM.CourseID);
                map.RubricID = Convert.ToInt16(mappingVM.RubricID);
                map.ProgramID = Convert.ToInt16(mappingVM.ProgramID);

                //assigns current date to mapping, then adds it to the database
                map.CreatedDateTime = DateTime.Now;
                map.CreatedByLoginID = UserData.PersonId;
                db.ProgramAssessmentMappings.Add(map);
                db.SaveChanges();

            }
            else
            {
                {
                    //return Index(mappingViewModel);
                    return RedirectToAction("Index", "AdminMapping");
                }
            }

            //return Index(mappingViewModel);
            return RedirectToAction("Index", "AdminMapping");
        }

        //removes a mapping
        //[HttpPost]
        //public ActionResult DeleteMapping(MappingViewModel mappingVM)
        //{
        //    TempData["ProgramID"] = mappingVM.ProgramID;
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            db.ProgramAssessmentMappings.Remove(mappingVM.Mapping);
        //            db.SaveChanges();
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.WriteLine(e.Message);
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "AdminMapping");

        //    }

        //    return RedirectToAction("Index", "AdminMapping");
        //}




        [ActionName("DeleteMapping")]
        [HttpPost]
        public ActionResult DeleteMapping(MappingViewModel mappingViewModel, int mapID)
        {

            if (ModelState.IsValid)
            {
                var mapping = db.ProgramAssessmentMappings.Where(m => m.ProgramAssessmentMappingID == mapID).FirstOrDefault();
                try
                {
                    db.ProgramAssessmentMappings.Remove(mapping);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
                TempData["ProgramID"] = mappingViewModel.ProgramID;

            }
            //ProgramAssessmentMapping pamId = db.ProgramAssessmentMappings.Find(mapId);
            //int programId = pamId.ProgramID;
            //db.ProgramAssessmentMappings.Remove(pamId);
            //db.SaveChanges();
            //TempData["ProgramID"] = (int)programId;
            return RedirectToAction("Index", "AdminMapping");
        }


        private vmProgram LoadVmProgram(int? programID)
        {
            var result = (from p in db.Programs
                          where p.ProgramID == programID
                          select new vmProgram()
                          {
                              ProgramID = p.ProgramID,
                              Name = p.Name,
                              Number = p.Number,
                              Assessments = p.Assessments.Where(a1 => a1.IsActive == true).Select(a => new vmAssessment()
                              {
                                  Name = a.Name,
                                  AssessmentID = a.AssessmentID,
                                  RubricAssessments = a.RubricAssessments.Where(ra1 => ra1.AssessmentRubric.IsActive == true).Select(ra => new vmRubricAssessment()
                                  {
                                      AssessmentRubric = new vmAssessmentRubric()
                                      {
                                          Name = ra.AssessmentRubric.Name,
                                          ProgramAssessmentMappings = ra.AssessmentRubric.ProgramAssessmentMappings.Select(pam => new vmProgramAssessmentMapping()
                                          {
                                              ProgramAssessmentMappingID = pam.ProgramAssessmentMappingID,
                                              Course = new vmCourse() { CourseName = pam.Course.CourseName, Number = pam.Course.Number }
                                          }).ToList()
                                      }
                                  }).ToList()
                              }).ToList()
                          }).FirstOrDefault();

            var institutionWideAssessments = (from a in db.Assessments
                                              where a.ProgramID == 0
                                                && a.IsActive == true
                                              select new vmAssessment()
                                              {
                                                  Name = a.Name,
                                                  AssessmentID = a.AssessmentID,
                                                  RubricAssessments = a.RubricAssessments.Where(ra1 => ra1.AssessmentRubric.IsActive == true).Select(ra => new vmRubricAssessment()
                                                  {
                                                      AssessmentRubric = new vmAssessmentRubric()
                                                      {
                                                          Name = ra.AssessmentRubric.Name,
                                                          ProgramAssessmentMappings = ra.AssessmentRubric.ProgramAssessmentMappings.Where(pam1 => pam1.ProgramID == programID).Select(pam => new vmProgramAssessmentMapping()
                                                          {
                                                              ProgramAssessmentMappingID = pam.ProgramAssessmentMappingID,
                                                              Course = new vmCourse() { CourseName = pam.Course.CourseName, Number = pam.Course.Number }
                                                          }).ToList()
                                                      }
                                                  })
                                              }).ToList();
            var tmpList = result.Assessments.ToList();
            tmpList.AddRange(institutionWideAssessments);

            result.Assessments = tmpList;

            return result;
        }
    }

}