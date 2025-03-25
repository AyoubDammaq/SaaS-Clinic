using Microsoft.AspNetCore.Mvc;

namespace DoctorManagementService.Controllers
{
    public class MedecinService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
