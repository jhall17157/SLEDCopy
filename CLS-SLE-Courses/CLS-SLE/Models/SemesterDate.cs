//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CLS_SLE.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SemesterDate
    {
        public short SemesterID { get; set; }
        public Nullable<int> SemesterCode { get; set; }
        public string SemesterDescription { get; set; }
        public string YearCode { get; set; }
        public string TermCode { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
    }
}
