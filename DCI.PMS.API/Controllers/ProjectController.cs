using Microsoft.AspNetCore.Mvc;

namespace DCI.PMS.API.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
