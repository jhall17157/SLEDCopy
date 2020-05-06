using CLS_SLE.Models;
using System;
using System.Collections.Generic;
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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}