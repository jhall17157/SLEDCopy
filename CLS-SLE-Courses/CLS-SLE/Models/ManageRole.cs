using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLS_SLE.Models
{
    public class ManageRole
    {
        public ManageRole(short ri, string rn, List<Permission> p, List<RolePermission> rp)
        {
            this.RoleID = ri;
            this.RoleName = rn;
            this.Permissions = p;
            this.RolePermissions = rp;
        }

        public short RoleID { get; set; }
        public string RoleName { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<RolePermission> RolePermissions { get; set; }
    }
}