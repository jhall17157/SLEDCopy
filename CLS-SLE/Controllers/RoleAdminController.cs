using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CLS_SLE.Models;

namespace CLS_SLE.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class RoleAdminController : Controller
    {
        SLE_TrackingEntities db = new SLE_TrackingEntities();
        public ActionResult Index() => View(db.Roles);
    }
}