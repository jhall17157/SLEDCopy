using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class RoleMembershipUserModel
    {
        public string login { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string id { get; set; }
        public int PID { get; set; }
        public string listView { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
    }
}