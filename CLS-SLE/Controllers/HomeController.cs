using System.Web.Mvc;
using System.Web.Security;

namespace CLS_SLE.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return RedirectToAction(actionName: "Dashboard", controllerName: "InstructorAssessments");
        }

        //GET: Home/SignOut
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction(actionName: "SignIn", controllerName: "User");
        }

        //GET: Home/CheckEmail
        public ActionResult CheckEmail()
        {
            return View();
        }
    }
}