using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CLS_SLE.Models;

namespace CLS_SLE.ViewModels
{
    public class InsertNewAssesmentViewModel
    {
        public Assessment Assessment { get; set; }
        public List<AssessmentCategory> AssessmentCategories { get; set; }
        public List<Program> Programs { get; set; }
        private readonly SLE_TrackingEntities DB = new SLE_TrackingEntities();

        InsertNewAssesmentViewModel()
        {
            Programs = (from programs in DB.Programs
                        orderby programs.Name
                        select programs).ToList();

            AssessmentCategories = (from Categories in DB.AssessmentCategories
                                    orderby Categories.Name
                                    select Categories).ToList();
        }
    }
}