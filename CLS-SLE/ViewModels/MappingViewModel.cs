using CLS_SLE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.ViewModels
{
    //New view model for mappings
    public class MappingViewModel
    {
        public SelectList AvailablePrograms { get; set; }
        public int? SelectedProgram { get; set; }
        public IEnumerable<Program> Programs { get; set; }
        public Program Program { get; set; }
        
        public IEnumerable<Course> Courses { get; set; }
       
        public ProgramAssessmentMapping Mapping { get; set; }

        
    }
}