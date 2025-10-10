using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;

namespace Quanlythuvien.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public StudentsController(QuanlythuvienDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách sách có sẵn
            var availableBooks = await _context.Books
                .Where(b => b.Quantity > 0)
                .Take(10)
                .ToListAsync();
            ViewBag.AvailableBooks = availableBooks;

            return View();
        }

        // Đăng ký mượn sách
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            if (HttpContext.Session.GetString("UserRole") != "Student")
            {
                return RedirectToAction("Login", "Account");
            }

            var book = await _context.Books.FindAsync(bookId);
            if (book != null && book.Quantity > 0)
            {

                book.Quantity -= 1;
                var borrow = new Borrowed
                {
                    StudentId = 1,
                    BookId = bookId,
                    BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                    DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
                };
                _context.Borroweds.Add(borrow);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Đăng ký mượn sách thành công!";
            }
            else
            {
                TempData["Message"] = "Sách không khả dụng.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,Username,PasswordHash,Fullname,Email,Phone,Address,Status")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,Username,PasswordHash,Fullname,Email,Phone,Address,Status")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
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
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
