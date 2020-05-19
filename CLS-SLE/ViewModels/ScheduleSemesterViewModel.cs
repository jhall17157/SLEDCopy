using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CLS_SLE.ViewModels
{
    public class ScheduleSemesterViewModel
    {
        public List<SelectListItem> Semesters { get; set; }
        public int SemesterID { get; set; }
        public Semester Semester { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public int StartDays { get; set; }
        public int EndDays { get; set; }
        public bool isDates { get; set; }
    }
}