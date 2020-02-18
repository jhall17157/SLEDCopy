using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.Models;

namespace CLS_SLE.ViewModels
{
    //New view model for mappings
    public class MappingViewModel
    {
        public SelectList AvailablePrograms { get; set; }
        public int? SelectedProgram { get; set; }
        public IEnumerable<Program> Programs { get; set; }
        public Program Program { get; set; }
        public IEnumerable<Assessment> Assessments { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        //rubrics
        public IEnumerable<AssessmentRubric> Rubrics { get; set; }

        //public IEnumerable<ProgramAssessmentMapping> ProgramAssessmentMappings { get; set; }

        

        public static MappingViewModel GetProgramModel(int programID)
        {
            using (SLE_TrackingEntities db = new SLE_TrackingEntities())
            {
                


                MappingViewModel Model = new MappingViewModel()
                {
                    Programs = db.Programs,

                    Program = db.Programs.Where(p => p.ProgramID == programID).FirstOrDefault(),
                    //ProgramAssessmentMappings = db.ProgramAssessmentMappings.Where(p => p.ProgramID == programID).ToList(),

                    Assessments = db.Assessments.Where(a => a.ProgramID == programID).ToList()



                    //Courses = db.Courses.OrderBy(c => c.CourseID),
                    //Rubrics = db.AssessmentRubrics

                };

                //Rubrics = db.AssessmentRubrics.Where(r => r.RubricID in Model.Assessments.ToList().ForEach(a => .)) 
                    
                    
                    
                //    Model.Assessments.ToList().ForEach(a =>

                //)

                //Model.AvailablePrograms = new SelectList(Model.Programs, "ProgramID", "Name");
                return Model;
            }
        }

    }
}