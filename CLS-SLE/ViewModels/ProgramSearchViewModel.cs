using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class ProgramSearchViewModel
    {
        public IEnumerable<Program> Programs { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public String SearchInput { get; set; }
        public string DepartmentFilter { get; set; }
    }
}