using CLS_SLE.Models;
using System.Collections.Generic;
using System.Linq;

namespace CLS_SLE.ViewModels
{
    public class ViewUserViewModel
    {
        public User User { get; set; }
        public List<UserSecurity> UserSecurities;
        public IQueryable<Role> Roles;

    }
}