using Microsoft.AspNetCore.Mvc;

namespace DCI.PMS.WebApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
