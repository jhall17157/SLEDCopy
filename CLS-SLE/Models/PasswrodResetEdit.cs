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
        public string email { get; set; }
        [Required]
        public string PWResetKey { get; set; }
        [Required]
        public string Hash { get; set; }
        [Required]
        public string SecondHash { get; set; }
    }
}