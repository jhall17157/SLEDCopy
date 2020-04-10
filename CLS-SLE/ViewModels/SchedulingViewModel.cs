using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CLS_SLE.Models;

namespace CLS_SLE.ViewModels
{
    public class SchedulingViewModel
    {
        public List<SelectListItem> Semesters { get; set; }
        public int SemesterID { get; set; }
        public Semester Semester { get; set; }
        
        public Section Section { get; set; }
        //public IQueryable<Person> People { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}