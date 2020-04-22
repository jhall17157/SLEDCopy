using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class UpdateSectionViewModel
    {
        public Section Section { get; set; }
        public IEnumerable<String> SemesterNames { get; set; }
        public String SemesterSelection { get; set; }
        public bool IsCancelled { get; set; }
    }
}