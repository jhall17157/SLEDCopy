namespace CLS_SLE.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class InstructorAssessment
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(100)]
        public string Login { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PersonID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short CourseID { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(150)]
        public string CourseName { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SectionID { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CRN { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(200)]
        public string AssessmentName { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short RubricID { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(200)]
        public string RubricName { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(1)]
        public string AssessmentLevel { get; set; }

        [Key]
        [Column(Order = 10, TypeName = "date")]
        public DateTime DueDate { get; set; }

        [Key]
        [Column(Order = 11)]
        [StringLength(1)]
        public string Status { get; set; }
    }
}
