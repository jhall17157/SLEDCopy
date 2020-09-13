using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.ViewModels
{
    public class AddSectionViewModel
    {
        public Section Section { get; set; }
        public List<String> SemesterList { get; set; }
        public String SemesterSelection { get; set; }
        public List<String> LeadInstructorList { get; set; }
        public String LeadInstructorSelection { get; set; }
    }
}