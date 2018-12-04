using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CLS_SLE.Models
{
    public class UserChangePassword
    {
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DisplayName("Confirm Password")]
        [Required, DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}