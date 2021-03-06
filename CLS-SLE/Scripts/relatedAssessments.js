$(document).ready(function()
{
	$("#addRelatedAssessment").click(function()
	{
		var id = parseInt($(".relatedAssessment").last().children("select").attr("name").toString().substr(17)) + 1;

		if ($(".relatedAssessment").length === 1)
			$(".relatedAssessment").append("<div class=\"input-group-append\"><button type=\"button\" class=\"btn btn-danger deleteRelatedAssessment\"><i class=\"fas fa-trash\"></i></button></div>");

        $(".relatedAssessment").last().clone().children("select").attr("name", "relatedAssessment" + id).parent().appendTo("#relatedAssessments")
		//$("#relatedAssessments").append("<div class=\"input-group relatedAssessment\"><select name=\"relatedAssessment" + id + "\" class=\"form-control\"><div class=\"input-group-append\"><button type=\"button\" class=\"btn btn-danger deleteRelatedAssessment\"><i class=\"fas fa-trash\"></i></button></div></div>");
	});

	$("#relatedAssessments").on("click", ".deleteRelatedAssessment", function()
	{
		$(this).prev().remove();

		$(this).parents(".relatedAssessment").remove();

		if ($(".relatedAssessment").length === 1)
			$(".deleteRelatedAssessment").parent().remove();
	});
});