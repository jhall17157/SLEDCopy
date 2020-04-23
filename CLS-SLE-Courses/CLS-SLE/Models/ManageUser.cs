using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.Models
{
    public class ManageUser
    {
        public ManageUser(int personID, string login, string iDNumber, string firstName, string lastName, List<Role> roles, List<UserRole> userRoles)
        {
            PersonID = personID;
            Login = login;
            IDNumber = iDNumber;
            FirstName = firstName;
            LastName = lastName;
            Roles = roles;
            UserRoles = userRoles;
        }

        public int PersonID { get; set; }
        public String Login { get; set; }
        public String IDNumber { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public List<Role> Roles { get; set; }
        public List<UserRole> UserRoles { get; set; }

    }
}