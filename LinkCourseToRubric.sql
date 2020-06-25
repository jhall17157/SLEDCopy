select  AR.RubricID, AR.Name,
	    C.CourseID, C.CourseName

From ProgramAssessmentMapping PAM 
Join AssessmentRubric AR on AR.RubricID = PAM.RubricID
Join Course C on C.CourseID = PAM.CourseID
