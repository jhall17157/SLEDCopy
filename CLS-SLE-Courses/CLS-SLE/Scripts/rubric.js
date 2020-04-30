console.log("running");
$(document).ready(function()
{
	$(".outcomeDetails, .outcomeCriterion, .criterionDetails").hide();

	$(".outcomeDetailsButton").click(function()
	{
		var outcomeID = $(this).closest("tbody").attr("id").substr(7);

		if ($("#outcomeDetails" + outcomeID).is(":visible"))
		{
			$("#outcomeDetails" + outcomeID).hide();
		}
		else
		{
			$(".outcomeDetails, .outcomeCriterion").hide();
			$("#outcomeDetails" + outcomeID).show();
		}
	});

	$(".outcomeCriterionButton").click(function()
	{
		var outcomeID = $(this).closest("tbody").attr("id").substr(7);

		if ($(".outcomeCriterion" + outcomeID).is(":visible"))
		{
			$(".outcomeCriterion" + outcomeID).hide();
		}
		else
		{
			$(".outcomeDetails, .outcomeCriterion").hide();
			$(".outcomeCriterion" + outcomeID).filter(".criterion").show();
		}
	});

	$(".criterionDetailsButton").click(function()
	{
		var criterionID = $(this).closest("tbody").attr("id").substr(9);

		if ($("#criterionDetails" + criterionID).is(":visible"))
		{
			$("#criterionDetails" + criterionID).hide();
		}
		else
		{
			$(".criterionDetails").hide();
			$("#criterionDetails" + criterionID).show();
		}
	});
});