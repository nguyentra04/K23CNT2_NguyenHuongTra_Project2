using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quanlythuvien.Models;

namespace Quanlythuvien.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly QuanlythuvienDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, QuanlythuvienDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        // Lấy vai trò của người dùng
                        var roles = await _userManager.GetRolesAsync(user);
                        var role = roles.FirstOrDefault();

                        // Lưu vai trò và username vào session
                        HttpContext.Session.SetString("UserRole", role ?? "Student"); 
                        HttpContext.Session.SetString("Username", username);

                        // Chuyển hướng dựa trên vai trò
                        switch (role)
                        {
                            case "Admin":
                                return RedirectToAction("Index", "Admin");
                            case "Librarian":
                                return RedirectToAction("Index", "Librarian");
                            case "Student":
                                return RedirectToAction("Index", "Student");
                            default:
                                return RedirectToAction("Index", "Home");
                        }
                    }
                }
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }
            return View();
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register (Tùy chọn, chỉ Admin có thể tạo người dùng mới)
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password, string role)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = username, Email = $"{username}@example.com" };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    return RedirectToAction("Index", "Admin");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

            [HttpPost]
            public IActionResult Logout(string returnUrl = null)
            {
                // Xóa session
                HttpContext.Session.Clear();

                // Đặt thông báo thành công
                TempData["Message"] = "Bạn đã đăng xuất thành công!";

                // Chuyển hướng về trang chủ hoặc URL được chỉ định
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
        }
    }

