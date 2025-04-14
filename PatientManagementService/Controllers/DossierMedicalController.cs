using Microsoft.AspNetCore.Mvc;

namespace PatientManagementService.Controllers
{
    public class DossierMedicalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
