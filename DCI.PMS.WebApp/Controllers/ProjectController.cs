using Microsoft.AspNetCore.Mvc;

namespace DCI.PMS.WebApp.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
