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
    
    public partial class RolePermission
    {
        public short RolePermissionID { get; set; }
        public short RoleID { get; set; }
        public short PermissionID { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<int> CreatedByLoginID { get; set; }
    
        public virtual Permission Permission { get; set; }
        public virtual Role Role { get; set; }
    }
}
