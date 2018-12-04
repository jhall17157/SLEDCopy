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
        [Required]
        public string Login { get; set; }
        [Required]
        public string PWResetKey { get; set; }
        [Required(ErrorMessage = "Password Change Could Not Be Completed"), 
        PasswordPropertyText, MinLength(8), MaxLength(16)]
        public string Hash { get; set; }
        [Required]
        public string SecondHash { get; set; }
    }
}