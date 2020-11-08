using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class ManageRolesViewModel
    {
        //spublic List<Role> Roles { get; set; }
        public List<Role> DeletableRoles { get; set; }
        public List<Role> NonDeletableRoles { get; set; }
        public String SearchTerm { get; set; }
    }
}