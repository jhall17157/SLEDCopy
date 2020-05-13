using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class SectionDetailViewModel
    {
        public string CreatorLogin { get; set; }
        public string ModifierLogin { get; set; }
        public string sectionLeadInstructor { get; set; }
        public Section section { get; set; }
        public string sectionSemester { get; set; }
        public string sectionCourse { get; set; }
        public string newStudent { get; set; }
        public List<StudentModel> newStudents { get; set; }
    }
}