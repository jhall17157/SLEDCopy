using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
<<<<<<< HEAD
using System.Linq;
using CLS_SLE.Models;
using System.Data.SqlClient;
=======
using CLS_SLE.Models;
>>>>>>> 4ef599df14a7262d78438212fcd84b2146519606

namespace CLS_SLE.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class RoleAdminController : Controller
    {
        SLE_TrackingEntities db = new SLE_TrackingEntities();
        public ActionResult Index() => View(db.Roles);
<<<<<<< HEAD

        public ActionResult Create() => View();

        public ActionResult Confirm([Required]int id)
        {
            var role = db.Roles.Where(r => id == r.RoleID).SingleOrDefault();
            ViewBag.Id = id;
            ViewBag.Name = role.Name;

            return View();
        }
        

        [HttpPost]
        public async Task<ActionResult> Create([Required]string name, [Required]string desc)
        {
            if (ModelState.IsValid)
            {
                    var role = new Role()
                    {
                        Name = name,
                        Description = desc
                    };

                    db.Roles.Add(role);
                    db.SaveChanges();
            }
            return View("Index", db.Roles);
        }

        [HttpPost]
        public async Task<ActionResult> Delete([Required]short id)
        {
            if (ModelState.IsValid)
            {
                    var role = db.Roles.Where(r => id == r.RoleID).SingleOrDefault();
                    db.Roles.Remove(role);
                    db.SaveChanges();

            }
            return View("Index", db.Roles);
        }

=======
>>>>>>> 4ef599df14a7262d78438212fcd84b2146519606
    }
}