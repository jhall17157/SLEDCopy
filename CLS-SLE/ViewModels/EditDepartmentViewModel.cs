using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CLS_SLE.ViewModels
{
    public class EditDepartmentViewModel
    {
        public IEnumerable<String> SchoolNames { get; set; }
        public String SchoolSelection { get; set; }
        public Department Department { get; set; }
    }
}