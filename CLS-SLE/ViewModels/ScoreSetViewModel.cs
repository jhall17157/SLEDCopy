using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class ScoreSetViewModel
    {

        public string Name { get; set; }
        public bool IsActive { get; set; }
        public List<ScoreSet> ScoreSets { get; set; }
    }
}