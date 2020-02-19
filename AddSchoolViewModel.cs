using System;
using CLS_SLE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using NLog;

namespace CLS_SLE.ViewModels
{
    public class AddSchoolViewModel
    {
        public byte SchoolID { get; set; }
        [Required(ErrorMessage = "Please enter School name here!")]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int CreatedByLoginID { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int ModifiedByLoginID { get; set; }     
        public Department Departments { get; set; }
        public SchoolSecurity SchoolSecurities { get; set; }
        public virtual  List<School> Schools { get; set; }
        public School School { get; set; }

        ////checking for duplicate
        //public AddSchoolViewModel(School s)
        //{
        //    bool duplicateName = false;

        //    foreach (byte school  in Schools)
        //    {
        //        if (school.name == s.Name)
        //        {
        //            duplicateName = true;
        //        }
        //    }

        //    if (!duplicateName)
        //    {
        //        db.Schools.Add(School);
        //        db.SaveChanges();
        //        Logger.Info("Success - " + school.Name + " school added!");
        //    }
        //    else
        //    {
        //        //rejected
        //        logger.Info("Rejected - dupilicate school name!");
        //    }
        //}

        //public Department Department { get; set; }
        //public SchoolSecurity SchoolSecurity { get; set; }
        //public School()
        //{
        //    this.School.Departments = new HashSet<Department>();
        //    this.School.SchoolSecurities = new HashSet<SchoolSecurity>();
        //}
       
    }
}