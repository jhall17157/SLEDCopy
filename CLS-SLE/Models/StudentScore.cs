
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
    
public partial class StudentScore
{

    public int StudentScoreID { get; set; }

    public long EnrollmentID { get; set; }

    public short CriteriaID { get; set; }

    public int AssessedByID { get; set; }

    public Nullable<System.DateTime> DateTimeAssessed { get; set; }

    public short ScoreID { get; set; }



    public virtual Criterion Criterion { get; set; }

    public virtual Enrollment Enrollment { get; set; }

    public virtual Person Person { get; set; }

    public virtual Score Score { get; set; }

}

}
