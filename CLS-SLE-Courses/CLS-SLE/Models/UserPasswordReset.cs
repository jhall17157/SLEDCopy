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
        [Required(ErrorMessage = "Login Required")]
        [DisplayName("Login")]
        public string Login { get; set; }
        public string Email { get; set; }
        public string PWResetKey { get; set; }
        public DateTime PWKeySentTime { get; set; }
    }
}