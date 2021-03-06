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
        public List<Enrollment> enrolled { get; set; }
        public List<Enrollment> dropped { get; set; }
        public HttpPostedFileBase attachment { get; set; }
        public List<Enrollment> newEnrollments { get; set; }
        public List<Person> errorList { get; set; }
        public List<Person> mismatchList { get; set; }
    }
}