using LibraryManagement2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace LibraryManagement2.Controllers
{
    public class AccountController : Controller
    {
        // Kiểm tra xem session đã được cấu hình chưa, nếu chưa thì cấu hình
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            // Đảm bảo session được bật (cần cấu hình trong Startup.cs/Program.cs)
            if (_httpContextAccessor.HttpContext?.Session == null)
            {
                throw new InvalidOperationException("Session không được bật. Vui lòng cấu hình dịch vụ session.");
            }
        }

        // Hiển thị form đăng nhập
        // GET: /Account/Login
        public IActionResult Login()
        {
            // Kiểm tra nếu đã đăng nhập, chuyển hướng đến trang tương ứng
            var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
            if (!string.IsNullOrEmpty(userRole))
            {
                return RedirectToRoleIndex(userRole);
            }
            return View(); // Trả về view đăng nhập nếu chưa đăng nhập
        }

        // Xử lý đăng nhập khi form được submit
        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic demo hard-code (nên thay bằng cơ chế xác thực thực tế như ASP.NET Identity)
                if (model.Username == "admin" && model.Password == "123")
                {
                    _httpContextAccessor.HttpContext.Session.SetString("UserRole", "Admin");
                    return RedirectToAction("Index", "Admins");
                }
                else if (model.Username == "librarian" && model.Password == "123")
                {
                    _httpContextAccessor.HttpContext.Session.SetString("UserRole", "Librarian");
                    return RedirectToAction("Index", "Librarian");
                }
                else if (model.Username == "student" && model.Password == "123")
                {
                    _httpContextAccessor.HttpContext.Session.SetString("UserRole", "Student");
                    return RedirectToAction("Index", "Student");
                }
                else
                {
                    ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu!"); // Sử dụng ModelState thay ViewBag
                }
            }
            return View(model); // Trả về view với lỗi nếu không hợp lệ
        }

        // Xử lý đăng xuất
        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear(); // Xóa tất cả session
            return RedirectToAction("Login"); // Chuyển hướng về trang đăng nhập
        }

        // Phương thức hỗ trợ chuyển hướng dựa trên vai trò
        private IActionResult RedirectToRoleIndex(string role)
        {
            return role switch
            {
                "Admin" => RedirectToAction("Index", "Admins"),
                "Librarian" => RedirectToAction("Index", "Librarian"),
                "Student" => RedirectToAction("Index", "Student"),
                _ => RedirectToAction("Login") // Mặc định quay lại login nếu vai trò không hợp lệ
            };
        }
    }
}