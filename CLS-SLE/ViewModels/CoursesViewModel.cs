using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class CoursesViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<String> DepartmentNames { get; set; }
        public string updatedMessage { get; set; }
        public string alertMessage { get; set; }
        public List<String> ERPList { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string SearchInput { get; set; }
        public string DepartmentFilter { get; set; }
        public string ERPFilter { get; set; }
        
    }
}