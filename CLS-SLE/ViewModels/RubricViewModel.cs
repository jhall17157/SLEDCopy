using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class RubricViewModel
    {
        public AssessmentRubric AssessmentRubric { get; set; }
        public RubricAssessment RubricAssesssment { get; set; }

        public int AssessmentID { get; set; }
        public List<string> AssessmentList { get; set; }
    }
}