/**/

Select 
	P.ProgramID, 
	P.Name as 'Program Name',
	C.CourseID, 
	C.CourseName as 'Course Name'

From ProgramAssessmentMapping PAM 
Join Course  C ON C.CourseID = PAM.COURSEID
Join Program P ON PAM.ProgramID = P.ProgramID


/*
Select *
From Program

Select *
From Course

Select *
From ProgramAssessmentMapping



Select *
From AssessmentRubric 
*/