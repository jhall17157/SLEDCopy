using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.Models
{
    public class UserSecurity
    {
        public UserSecurity(int personID, string iDNumber, string firstName, string lastName, List<Role> roles)
        {
            PersonID = personID;
            IDNumber = iDNumber;
            FirstName = firstName;
            LastName = lastName;
            Roles = roles;
        }

        public int PersonID { get; set; }
        public String IDNumber { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public List<Role> Roles { get; set; }

    }
}