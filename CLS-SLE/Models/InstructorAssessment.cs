
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
    
public partial class InstructorAssessment
{

    public string Login { get; set; }

    public int PersonID { get; set; }

    public short CourseID { get; set; }

    public string CourseName { get; set; }

    public int SectionID { get; set; }

    public int CRN { get; set; }

    public string AssessmentName { get; set; }

    public string Category { get; set; }

    public string CategoryName { get; set; }

    public short RubricID { get; set; }

    public string RubricName { get; set; }

    public string AssessmentLevel { get; set; }

    public System.DateTime DueDate { get; set; }

    public Nullable<int> StudentCount { get; set; }

    public Nullable<int> CompletedCount { get; set; }

}

}
