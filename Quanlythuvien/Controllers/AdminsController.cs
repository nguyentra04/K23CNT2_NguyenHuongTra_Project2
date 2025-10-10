using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;

namespace Quanlythuvien.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public AdminsController(QuanlythuvienDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            // Kiểm tra quyền Admin
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            // Thống kê
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalStudents = await _context.Students.CountAsync();
            ViewBag.TotalLibrarians = await _context.Librarians.CountAsync();
            ViewBag.TotalBorrows = await _context.Borroweds.CountAsync(b => b.ReturnDate == null);

            return View();
        }

        // Quản lý Admin khác
        public IActionResult ManageAdmins() => RedirectToAction("Index", "Admins");

        // Quản lý Thủ Thư
        public IActionResult ManageLibrarians() => RedirectToAction("Index", "Librarians");

        // Quản lý Sinh Viên
        public IActionResult ManageStudents() => RedirectToAction("Index", "Students");

        // Quản lý Sách
        public IActionResult ManageBooks() => RedirectToAction("Index", "Books");

        // Xem thống kê sách mượn nhiều
        public async Task<IActionResult> BorrowStats()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var topBooks = await _context.Borroweds
                .Include(b => b.Book)
                .GroupBy(b => b.BookId)
                .Select(g => new { Book = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();
            return View(topBooks);
        }

        // Xem phiếu mượn của sinh viên
        public async Task<IActionResult> StudentBorrows(int studentId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var borrows = await _context.Borroweds
                .Include(b => b.Student)
                .Include(b => b.Book)
                .Where(b => b.StudentId == studentId)
                .ToListAsync();
            return View(borrows);
        }



        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền Admin
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            return View(admin);
        }

        // GET: Admins/Create
        public IActionResult Create()
        {
            // Kiểm tra quyền Admin
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Admins/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,Username,PasswordHash,Fullname,Email,CreatedAt")] Admin admin)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra email hợp lệ (cơ bản)
                if (!IsValidEmail(admin.Email))
                {
                    ModelState.AddModelError("Email", "Email không hợp lệ.");
                    return View(admin);
                }

                // Đặt CreatedAt mặc định nếu không có
                if (admin.CreatedAt == default)
                {
                    admin.CreatedAt = DateTime.Now;
                }

                // Kiểm tra PasswordHash không rỗng (nên dùng Identity để mã hóa)
                if (string.IsNullOrEmpty(admin.PasswordHash))
                {
                    ModelState.AddModelError("PasswordHash", "Mật khẩu không được để trống.");
                    return View(admin);
                }

                _context.Add(admin);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Thêm Admin thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền Admin
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            return View(admin);
        }

        // POST: Admins/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,Username,PasswordHash,Fullname,Email,CreatedAt")] Admin admin)
        {
            if (id != admin.AdminId)
            {
                return NotFound();
            }

            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra email hợp lệ
                    if (!IsValidEmail(admin.Email))
                    {
                        ModelState.AddModelError("Email", "Email không hợp lệ.");
                        return View(admin);
                    }

                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Cập nhật Admin thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền Admin
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Xóa Admin thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminId == id);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}