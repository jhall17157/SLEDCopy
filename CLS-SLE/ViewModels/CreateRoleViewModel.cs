using CLS_SLE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.ViewModels
{
    public class CreateRoleViewModel
    {
        public Role role { get; set; }

        public CreateRoleViewModel()
        {
            role = new Role();
        }
    }
}