using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CLS_SLE.Models;

namespace CLS_SLE.ViewModels
{
    public class AddUserViewModel
    {
        public User User { get; set; }
        public Person Person { get; set; }

        public void HashStudentID(string unHashedId)
        {
             this.User.Hash = BCrypt.Net.BCrypt.HashString(unHashedId);
        }
    }
}