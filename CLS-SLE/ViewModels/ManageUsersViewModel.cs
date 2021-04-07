using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class ManageUsersViewModel
    {
        public List<User> Users { get; set; }
        //public List<UserSecurity> UserSecurities;
        //public IQueryable<Role> Roles;
        public String SearchTerm { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}