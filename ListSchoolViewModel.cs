using System;
using CLS_SLE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class ListSchoolViewModel
    {
        public School School { get; set; }
        public byte SchoolID { get; set; }
        public List<byte> Schools { get; set; }
    }
}