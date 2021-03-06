
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
    
public partial class User
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public User()
    {

        this.UserRoles = new HashSet<UserRole>();

    }


    public int PersonID { get; set; }

    public string Login { get; set; }

    public string Email { get; set; }

    public Nullable<System.DateTime> LastLogin { get; set; }

    public bool MustResetPassword { get; set; }

    public string Hash { get; set; }

    public string TemporaryPasswordHash { get; set; }

    public Nullable<System.DateTime> TemporaryPasswordIssued { get; set; }

    public bool IsActive { get; set; }

    public Nullable<System.DateTime> CreatedDateTime { get; set; }

    public Nullable<int> CreatedByLoginID { get; set; }

    public Nullable<System.DateTime> ModifiedDateTime { get; set; }

    public Nullable<int> ModifiedByLoginID { get; set; }



    public virtual Person Person { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<UserRole> UserRoles { get; set; }

}

}
