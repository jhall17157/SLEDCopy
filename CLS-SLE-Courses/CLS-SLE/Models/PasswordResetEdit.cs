using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CLS_SLE.Models

{
    public class PasswordResetEdit
    {
        [Required(ErrorMessage = "Login Required")]
        [DisplayName("Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Code Required")]
        [DisplayName("Unique Code")]
        public string PWResetKey { get; set; }

        [Required(ErrorMessage = "Password Required")]
        [PasswordPropertyText]
        [StringLength(16, MinimumLength = 8)]
        [DisplayName("Password")]
        public string Hash { get; set; }

        [Required(ErrorMessage = "Confirmation Password Required")]
        [PasswordPropertyText]
        [StringLength(16, MinimumLength = 8)]
        [DisplayName("Confirm Password")]
        [Compare("Hash", ErrorMessage = "Passwords Do Not Match")]
        public string SecondHash { get; set; }
    }
}