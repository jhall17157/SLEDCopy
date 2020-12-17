
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
    
public partial class Semester
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Semester()
    {

        this.Sections = new HashSet<Section>();

        this.StudentPrograms = new HashSet<StudentProgram>();

    }


    public short SemesterID { get; set; }

    public Nullable<int> SemesterCode { get; set; }

    public string Name { get; set; }

    public Nullable<System.DateTime> CreatedDateTime { get; set; }

    public Nullable<int> CreatedByLoginID { get; set; }

    public Nullable<System.DateTime> ModifiedDateTime { get; set; }

    public Nullable<int> ModifiedByLoginID { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Section> Sections { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<StudentProgram> StudentPrograms { get; set; }

}

}
