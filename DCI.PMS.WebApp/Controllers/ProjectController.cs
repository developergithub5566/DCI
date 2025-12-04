using Microsoft.AspNetCore.Mvc;

namespace DCI.PMS.WebApp.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateEdit()
        {
            return View();
        }

        public IActionResult Milestone()
        {
            return View();
        }

        public IActionResult Deliverables()
        {
            return View();
        }
    }
}
