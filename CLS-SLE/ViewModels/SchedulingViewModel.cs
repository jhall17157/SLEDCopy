using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.ViewModels
{
    //select lists could be generated in the cshtml page instead, if desired. Shouldn't impact performance much either way. 
    public class SchedulingViewModel
    {
        public String searchTerm { get; set; }
        public List<SelectListItem> Semesters { get; set; }
        public int SemesterID { get; set; }
        public Semester Semester { get; set; }
        public List<Course> Courses { get; set; }
        public List<SelectListItem> CourseSelectList { get; set; }
        public List<SelectListItem> CourseForAddEntry { get; set; }
        public int? CourseID { get; set; }
        public Course CourseSearch { get; set; }
        public Section Section { get; set; }
        public List<SelectListItem> Sections { get; set; }
        public List<SelectListItem> SectionRubrics { get; set; }

        public int SectionID { get; set; }
        public int RubricID { get; set; }

        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public String FocusIDCRN { get; set; }
        public String FocusIDCourse { get; set; }

        public List<SelectListItem> AssesmentRubrics { get; set; }


    }
}