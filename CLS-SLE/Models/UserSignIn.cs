using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLS_SLE.Models
{
    public class UserSignIn
    {
        public string Login { get; set; }
        [Required(ErrorMessage = "Hash Required")]
        public string Hash { get; set; }
    }
}