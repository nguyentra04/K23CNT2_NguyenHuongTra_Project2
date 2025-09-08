using LibraryManagement2.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement2.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Demo hard-code phân quyền
                if (model.Username == "admin" && model.Password == "123")
                {
                    HttpContext.Session.SetString("UserRole", "Admin");
                    return RedirectToAction("Index", "Admin");
                }
                else if (model.Username == "librarian" && model.Password == "123")
                {
                    HttpContext.Session.SetString("UserRole", "Librarian");
                    return RedirectToAction("Index", "Librarian");
                }
                else if (model.Username == "student" && model.Password == "123")
                {
                    HttpContext.Session.SetString("UserRole", "Student");
                    return RedirectToAction("Index", "Student");
                }
                else
                {
                    ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

