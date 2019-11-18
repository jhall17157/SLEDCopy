using System.Collections.Generic;

namespace CLS_SLE.Models
{
    public class AssessmentSchedulingViewModel
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
    }
}