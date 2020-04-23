﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CLS_SLE.Models
{
    public class UserSignIn
    {
        [Required(ErrorMessage = "Login Required")]
        [DisplayName("Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [PasswordPropertyText]
        [DisplayName("Password")]
        public string Hash { get; set; }

    }
}