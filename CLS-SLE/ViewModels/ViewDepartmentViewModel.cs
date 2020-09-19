using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class ViewDepartmentViewModel
    {
        public Department Department { get; set; }
        public string updatedMessage { get; set; }
        public string alertMessage { get; set; }
    }
}