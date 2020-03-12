using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class CreateCourseViewModel
    {
        public Course Course { get; set; }
        public String DepartmentSelection { get; set; }
    }
}