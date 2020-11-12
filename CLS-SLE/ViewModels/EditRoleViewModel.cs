using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class EditRoleViewModel
    {
        public Role role { get; set; }
        public List<SchoolSecurity> schoolSecurities { get; set; }
        public List<ProgramSecurity> programSecurities { get; set; }
        public List<DepartmentSecurity> departmentSecurities { get; set; }
        public List<School> allSchools { get; set; }
        public List<Department> allDepartments { get; set; }
        public List<Program> allPrograms { get; set; }
    }

    public class SchoolTruncated
    {
        public String Name { get; set; }
        public int SchoolID { get; set; }
    }

    public class ProgramTruncated
    {
        public String Name { get; set; }
        public int ProgramID { get; set; }
    }

    public class DepartmentTruncated
    {
        public String Name { get; set; }
        public int DepartmentID { get; set; }
    }
}