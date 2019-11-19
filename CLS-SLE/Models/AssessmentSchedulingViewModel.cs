using System.Collections.Generic;

namespace CLS_SLE.Models
{
    public class AssessmentSchedulingViewModel
    {
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<Section> Sections { get; set; }
        public IEnumerable<Semester> Semesters { get; set; }
        public IEnumerable<Program> Programs { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<AssessmentCategory> Categories { get; set; }
        public IEnumerable<Assessment> Assessments { get; set; }
        public IEnumerable<AssessmentRubric> AssessmentRubrics { get; set; }
        public IEnumerable<ProgramAssessmentMapping> ProgramAssessmentMappings { get; set; }
    }
}