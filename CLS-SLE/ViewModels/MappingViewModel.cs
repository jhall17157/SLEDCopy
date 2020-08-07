using CLS_SLE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.ViewModels
{
    //New view model for mappings
    public class MappingViewModel
    {
        
        public List<SelectListItem> Programs { get; set; }
        public int? ProgramID { get; set; }
        public vmProgram Program { get; set; }

        public List<SelectListItem> Courses { get; set; }
        public int CourseID { get; set; }
        public Course Course { get; set; }

        public List<SelectListItem> Rubrics { get; set; }
        public int RubricID { get; set; }

        public ProgramAssessmentMapping Mapping { get; set; }
        public int? MappingID { get; set; }
       
    }

    public class vmProgram
    {
        public vmProgram()
        {
            this.Assessments = new List<vmAssessment>();
        }

        public short ProgramID { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual IEnumerable<vmAssessment> Assessments { get; set; }

    }

    public class vmAssessment
    {
        public vmAssessment()
        {
            this.RubricAssessments = new List<vmRubricAssessment>();
        }

        public short AssessmentID { get; set; }
        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual IEnumerable<vmRubricAssessment> RubricAssessments { get; set; }

    }

    public partial class vmRubricAssessment
    {
        public int RubricAssessmentID { get; set; }
        public short RubricID { get; set; }
        public virtual vmAssessmentRubric AssessmentRubric { get; set; }
    }

    public partial class vmAssessmentRubric
    {
        public vmAssessmentRubric()
        {
            this.ProgramAssessmentMappings = new List<vmProgramAssessmentMapping>();
        }

        public string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual IEnumerable<vmProgramAssessmentMapping> ProgramAssessmentMappings { get; set; }

    }

    public partial class vmProgramAssessmentMapping
    {
        public short ProgramAssessmentMappingID { get; set; }
        public virtual vmCourse Course { get; set; }
    }

    public partial class vmCourse
    {
        public string CourseName { get; set; }
        public string Number { get; set; }
    }



}