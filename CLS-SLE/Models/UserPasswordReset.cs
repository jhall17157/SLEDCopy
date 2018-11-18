using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CLS_SLE.Models
{
    public class UserPasswordReset
    {
        public int IdNumber { get; set; }
        public string Login { get; set; }
        [Required(ErrorMessage = "Password Required")]
        public string Email { get; set; }
    }
}