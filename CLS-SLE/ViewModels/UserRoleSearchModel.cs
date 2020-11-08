using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class UserRoleSearchModel
    {
        public string login { get; set; }
        public int personID { get; set; }
        public string idNumber { get; set; }
        public string lastName { get; set; }

        public string firstName { get; set; }
    }
}