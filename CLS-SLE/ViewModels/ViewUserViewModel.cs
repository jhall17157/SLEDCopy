using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CLS_SLE.Models;

namespace CLS_SLE.ViewModels
{
    public class ViewUserViewModel
    {
        public User User { get; set; }
        public List<UserSecurity> UserSecurities;
        public IQueryable<Role> Roles;
   
    }
}