using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class ProgramListViewModel
    {
        public IEnumerable<Program> Programs { get; set; }
        public string updatedMessage { get; set; }
        public string alertMessage { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string SearchInput { get; set; }

    }
}