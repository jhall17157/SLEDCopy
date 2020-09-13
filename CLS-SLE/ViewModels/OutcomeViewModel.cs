using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CLS_SLE.Models;

namespace CLS_SLE.ViewModels
{
    public class OutcomeViewModel
    {
        public Outcome OutcomeVM { get; set; }
        public RubricAssessment Rubric { get; set; }
    }
}