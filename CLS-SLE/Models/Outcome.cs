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
    
    public partial class Outcome
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Outcome()
        {
            this.Criteria = new HashSet<Criterion>();
        }
    
        public short OutcomeID { get; set; }
        public short RubricID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte SortOrder { get; set; }
        public Nullable<decimal> CriteriaPassRate { get; set; }
        public bool CalculateCriteriaPassRate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<int> CreatedByLoginID { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        public Nullable<int> ModifiedByLoginID { get; set; }
        public Nullable<System.DateTime> InactiveDateTime { get; set; }
    
        public virtual AssessmentRubric AssessmentRubric { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Criterion> Criteria { get; set; }
    }
}
