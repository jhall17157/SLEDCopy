using CLS_SLE.Models;
using System.Collections.Generic;

namespace CLS_SLE.ViewModels
{
    public class UpdateUserViewModel
    {
        public User User { get; set; }
        public Person Person { get; set; }
        public List<string> Roles { get; set; }
        public List<UserRole> UserRoles { get; set; }
    }
}