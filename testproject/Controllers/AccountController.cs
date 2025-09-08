using Microsoft.AspNetCore.Mvc;

namespace testproject.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }
    }
}
