namespace CLS_SLE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RubricDetail
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(200)]
        public string AssessmentName { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short RubricID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(200)]
        public string RubricName { get; set; }

        public string Description { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(200)]
        public string OutcomeName { get; set; }

        [Key]
        [Column(Order = 4)]
        public byte OutcomeSortOrder { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CriteriaID { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(200)]
        public string CriteriaName { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(200)]
        public string ExampleText { get; set; }

        [Key]
        [Column(Order = 8)]
        public byte CriteriaSortOrder { get; set; }
    }
}
