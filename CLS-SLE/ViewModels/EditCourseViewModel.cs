using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class EditCourseViewModel
    {
        public IEnumerable<Department> Departments { get; set; }
        public Course Course { get; set; }
    }
}