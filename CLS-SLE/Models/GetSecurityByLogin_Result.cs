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
    
    public partial class GetSecurityByLogin_Result
    {
        public Nullable<byte> SchoolID { get; set; }
        public Nullable<short> DepartmentID { get; set; }
        public string DepartmentNumber { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<short> ProgramID { get; set; }
        public string ProgramNumber { get; set; }
        public string ProgramName { get; set; }
        public Nullable<bool> CanEdit { get; set; }
    }
}
