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
        public string Login { get; set; }
        [Required]
        public string Hash { get; set; }
        public string PWResetKey { get; set; }
        public DateTime PWKeySentTime { get; }
    }
}