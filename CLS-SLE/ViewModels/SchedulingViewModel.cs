using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CLS_SLE.ViewModels
{
    public class SchedulingViewModel
    {
        public List<SelectListItem> Semesters { get; set; }
        public int SemesterID { get; set; }
        public Semester Semester { get; set; }
        public List<Course> Courses { get; set; }
        public List<SelectListItem> CourseSelectList { get; set; }
        public int? CourseID { get; set; }
        public Course CourseSearch { get; set; }
        public Section Section { get; set; }
        public int SectionID { get; set; }
        public int RubricID { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PagingInfo PagingInfo { get; set; }

        public List<SelectListItem> AssesmentRubrics { get; set; }


    }
}