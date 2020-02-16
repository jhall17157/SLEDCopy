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
        //public SelectList Programs { get; set; }
        //public int? ProgramID { get; set; }
        public Program Program { get; set; }
        public IEnumerable<Assessment> Assessments { get; set; }


    }
}