using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CLS_SLE.Models;

namespace CLS_SLE.ViewModels
{
	public class CriterionViewModel
	{
		public Criterion Criterion { get; set; }
		public Outcome Outcome { get; set; }
		public RubricAssessment Rubric { get; set; }
	}
}