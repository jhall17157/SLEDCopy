namespace CLS_SLE.Models
{
    public class AssessmentLevelPair
    {
        public InstructorAssessment Assessment { get; set; }
        public string AssessmentLevel { get; set; }


        public AssessmentLevelPair(InstructorAssessment assessment, string assessmentLevel)
        {
            Assessment = assessment;
            AssessmentLevel = assessmentLevel;
        }
    }
}