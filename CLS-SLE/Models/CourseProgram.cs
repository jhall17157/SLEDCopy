
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
    
public partial class CourseProgram
{

    public short CourseProgramID { get; set; }

    public short CourseID { get; set; }

    public short ProgramID { get; set; }

    public Nullable<System.DateTime> StartDate { get; set; }

    public Nullable<System.DateTime> EndDate { get; set; }

    public Nullable<System.DateTime> CreatedDateTime { get; set; }

    public Nullable<int> CreatedByLoginID { get; set; }

    public Nullable<System.DateTime> ModifiedDateTime { get; set; }

    public Nullable<int> ModifiedByLoginID { get; set; }



    public virtual Course Course { get; set; }

    public virtual Program Program { get; set; }

}

}
