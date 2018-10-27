using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CLS_SLE.Models
{
    public class UserSignIn
    {
        [DisplayName("Username")]
        public string IdNumber { get; set; }
        [Required(ErrorMessage = "Password Required")]
        [DisplayName("Password")]
        public string Hash { get; set; }
    }
}