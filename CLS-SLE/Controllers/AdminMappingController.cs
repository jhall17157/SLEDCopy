using CLS_SLE.Models;
using CLS_SLE.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace CLS_SLE.Controllers
{

    //New controller for mapping page
    public class AdminMappingController : Controller
    {
        private SLE_TrackingEntities db = new SLE_TrackingEntities();


        // GET: AdminMapping home page with list of Programs sorted by id
        public ActionResult Index()
        {
            MappingViewModel mappingViewModel = new MappingViewModel();
            var availablePrograms = db.Programs;
            mappingViewModel.AvailablePrograms = new SelectList(availablePrograms, "ProgramID", "Name");
            return View(mappingViewModel);
        }

        //returns index page with viewmodel populated by selected program
        [HttpPost]
        public ActionResult Index(MappingViewModel model)
        {
            int programID = model.SelectedProgram.GetValueOrDefault();
            MappingViewModel mappingViewModel = new MappingViewModel();
            mappingViewModel.Programs = db.Programs;
            mappingViewModel.Program = mappingViewModel.Programs.Where(p => p.ProgramID == programID).FirstOrDefault();
            mappingViewModel.SelectedProgram = programID;
            mappingViewModel.AvailablePrograms = new SelectList(mappingViewModel.Programs, "ProgramID", "Name");
            return View(mappingViewModel);
        }



    }
}