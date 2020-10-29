﻿using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class AddProgramViewModel
    {
        public Program Program { get; set; }
        public IEnumerable<String> DepartmentNames { get; set; }
        public IEnumerable<String> DepartmentSelection { get; set; }
    }
}