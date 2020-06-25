using System.Collections.Generic;

namespace CLS_SLE.Models
{
    public class AssessmentMappingsViewModel
    {
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Program> Programs { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<AssessmentCategory> Categories { get; set; }
        public IEnumerable<Assessment> Assessments { get; set; }
        public IEnumerable<RubricAssessment> RubricAssessments { get; set; }
        public IEnumerable<AssessmentRubric> AssessmentRubrics { get; set; }
        public IEnumerable<ProgramAssessmentMapping> ProgramAssessmentMappings { get; set; }
        public IEnumerable<RubricsByProgram> RubricsByProgram { get; set; }
        public IEnumerable<AssessmentRubric> AssesmentRubrics { get; set; }

        /*
         IEnumerable<int> courseRubricLinq =
             from pam in ProgramAssessmentMapping
                      join ar in AssessmentRubric on pam.RubricID equals ar.RubricID
                      join  c in Course on pam.CourseID equals c.CourseID
                            select new
                                  {
                                    RubricID = pam.RubricID,
                                    CourseID = c.CourseID,
                                    RubricName = ar.Name,
                                    CourseName = c.CourseName
                                  }; 
                                  */
    }



}

