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
        public int PersonID { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public bool MustResetPassword { get; set; }
        public string Hash { get; set; }
    
        public virtual Person Person { get; set; }
    }
}
