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
    
    public partial class ProgramMap
    {
        public short ProgramID { get; set; }
        public string ProgramName { get; set; }
        public string ProgramNumber { get; set; }
        public short RubricID { get; set; }
        public string RubricName { get; set; }
        public Nullable<short> CourseID { get; set; }
        public string CourseNumber { get; set; }
        public string CourseName { get; set; }
        public string AssessmentLevelCode { get; set; }
        public string AssessmentLevel { get; set; }
        public Nullable<short> ProgramAssessmentMappingID { get; set; }
    }
}