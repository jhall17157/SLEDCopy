using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class SchoolDetailViewModel
    {
        public School School { get; set; }
        public IQueryable<Department> Departments;
    }
}