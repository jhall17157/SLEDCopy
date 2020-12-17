using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class ManageRoleMembershipViewModel
    {
        public List<UserSecurity> UsersInRole { get; set; }
        public int? RoleID { get; set; }
        public Role CurrentRole { get; set; }
        public String SearchTerm { get; set; }
    }
}