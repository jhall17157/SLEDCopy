using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class ScoreViewModel
    {
        public byte ScoreSetID { get; set; }
        public string Description { get; set; }
        public short Value { get; set; }
        public short SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int CreatedByLoginID { get; set; }
        public List<Score> scores { get; set; }
        public string ScoreSetName { get; set; }
        public Score Score { get; set; }
        public List<StudentScore> StudentScores { get; set; }
        public string message { get; set; }



    }
}