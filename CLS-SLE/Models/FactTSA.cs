
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
    
public partial class FactTSA
{

    public long TSAID { get; set; }

    public int StudentID { get; set; }

    public short ProgramID { get; set; }

    public string AcademicYear { get; set; }

    public string TSAStatus { get; set; }

    public byte OutcomesRequired { get; set; }

    public byte OutcomesAssessed { get; set; }

    public byte OutcomesMet { get; set; }

    public Nullable<decimal> PassRate { get; set; }

    public Nullable<decimal> TSAPercent { get; set; }

    public Nullable<System.DateTime> DateTSACompleted { get; set; }

    public bool TSAPassed { get; set; }

    public System.DateTime LastUpdate { get; set; }



    public virtual Person Person { get; set; }

    public virtual Program Program { get; set; }

}

}
