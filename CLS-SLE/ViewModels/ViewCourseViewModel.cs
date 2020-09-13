using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class ViewCourseViewModel
    {
        public Course course{ get; set; }
        public string updatedMessage { get; set; }
        public string alertMessage { get; set; }
        public string CreatorLogin { get; set; }
        public string ModifierLogin { get; set; }
        public List<Section> courseSections { get; set; }
    }
}