
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
    
public partial class StudentProgram
{

    public long StudentProgramID { get; set; }

    public int StudentID { get; set; }

    public short ProgramID { get; set; }

    public short SemesterID { get; set; }

    public bool IsPrimary { get; set; }

    public Nullable<System.DateTime> CreatedDateTime { get; set; }

    public Nullable<int> CreatedByLoginID { get; set; }

    public Nullable<System.DateTime> ModifiedDateTime { get; set; }

    public Nullable<int> ModifiedByLoginID { get; set; }



    public virtual Person Person { get; set; }

    public virtual Program Program { get; set; }

    public virtual Semester Semester { get; set; }

}

}
