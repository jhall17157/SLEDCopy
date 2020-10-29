using CLS_SLE.Models;
using System.Collections.Generic;

namespace CLS_SLE.ViewModels
{
    public class RubricViewModel
    {
        public AssessmentRubric AssessmentRubric { get; set; }
        public RubricAssessment RubricAssesssment { get; set; }

        public int AssessmentID { get; set; }
        public List<string> AssessmentList { get; set; }

        public List<ScoreSet> ScoreSetList { get; set; } 
    }
}