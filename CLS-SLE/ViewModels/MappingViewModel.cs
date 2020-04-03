using CLS_SLE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.ViewModels
{
    //New view model for mappings
    public class MappingViewModel
    {
        //public SelectList AvailablePrograms { get; set; }
        //public int? SelectedProgram { get; set; }
        //public IEnumerable<Program> Programs { get; set; }
        public List<SelectListItem> Programs { get; set; }
        public int ProgramID { get; set; }
        public Program Program { get; set; }

        public List<SelectListItem> Courses { get; set; }
        public int CourseID { get; set; }
        public Course Course { get; set; }

        public List<SelectListItem> Rubrics { get; set; }
        public int RubricID { get; set; }

        public ProgramAssessmentMapping Mapping { get; set; }
        public int? MappingID { get; set; }

        
    }
}