using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class UpdateProgramViewModel
    {
        public Program Program { get; set; }
        public bool IsActive { get; set; }
        public bool IsSharedProgram { get; set; }
    }
}