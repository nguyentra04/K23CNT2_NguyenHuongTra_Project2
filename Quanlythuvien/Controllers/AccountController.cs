﻿
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;

using System.Security.Claims;

namespace Quanlythuvien.Controllers
{
    public class AuthController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public AuthController(QuanlythuvienDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 1. Admin
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == username && a.PasswordHash == password);
            if (admin != null)
            {
                await SignInUser(admin.Username, "Admin");
                return RedirectToAction("Index", "Home"
                    );
            }

            // 2. Librarian
            var librarian = await _context.Librarians.FirstOrDefaultAsync(l => l.Username == username && l.PasswordHash == password);
            if (librarian != null)
            {
                await SignInUser(librarian.Username, "Librarian");
                return RedirectToAction("Index", "Home");
            }

            // 3. Student
            var student = await _context.Students
    .FirstOrDefaultAsync(s => s.Username == username && s.PasswordHash == password);
            if (student != null)
            {
                await SignInUser(student.Username, "Student");
                return RedirectToAction("Index", "Home");
            }


            ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
            return View();
        }

        private async Task SignInUser(string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied() => Content(" Bạn không có quyền truy cập!");


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Student model)
        {
            if (ModelState.IsValid)
            {
                var exist = await _context.Students.FirstOrDefaultAsync(s => s.Username == model.Username);
                if (exist != null)
                {
                    ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                    return View(model);
                }

                _context.Students.Add(model);
                await _context.SaveChangesAsync();

                await SignInUser(model.Username, "Student");

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}