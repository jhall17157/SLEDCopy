using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLS_SLE.Models
{
    public class UserSignIn
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }
    }
}