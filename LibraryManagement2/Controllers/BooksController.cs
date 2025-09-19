using LibraryManagement2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement2.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly ILogger<BooksController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BooksController(LibraryDbContext context, ILogger<BooksController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        // Hiển thị danh sách sách
        // GET: Books
        public async Task<IActionResult> Index()
        {
            try
            {
                var role = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
                if (string.IsNullOrEmpty(role) || role != "Admin")
                {
                    _logger.LogWarning("Truy cập không được phép vào Index tại {Time}", DateTime.Now);
                    return RedirectToAction("Login", "Account");
                }

                _logger.LogInformation("Truy cập danh sách sách tại {Time}", DateTime.Now);
                var books = await _context.Books.ToListAsync();
                return View(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy cập danh sách sách tại {Time}", DateTime.Now);
                return RedirectToAction("Error", "Home");
            }
        }

        // Hiển thị chi tiết một cuốn sách
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Yêu cầu Details với ID null tại {Time}", DateTime.Now);
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Cate)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                _logger.LogWarning("Không tìm thấy sách với ID {Id} tại {Time}", id, DateTime.Now);
                return NotFound();
            }

            return View(book);
        }

        // Hiển thị form tạo sách
        // GET: Books/Create
        public IActionResult Create()
        {
            var role = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                _logger.LogWarning("Truy cập không được phép vào Create tại {Time}", DateTime.Now);
                return RedirectToAction("Login", "Account");
            }

            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "Fullname");
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CateName");
            return View();
        }

        // Xử lý tạo sách khi form được submit
        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,Title,AuthorId,Publisher,YearPublished,CateId,Quantity,Description,Status,ImagePath")] Book book, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null)
                    {
                        var filePath = Path.Combine("wwwroot/images/books", ImageFile.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                        book.ImagePath = $"/images/books/{ImageFile.FileName}";
                    }

                    _context.Add(book);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Tạo sách thành công với ID {Id} tại {Time}", book.BookId, DateTime.Now);
                    TempData["Message"] = "Tạo sách thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tạo sách tại {Time}", DateTime.Now);
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo sách.");
                }
            }

            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "Fullname", book.AuthorId);
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CateName", book.CateId);
            return View(book);
        }

        // Hiển thị form chỉnh sửa sách
        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Yêu cầu Edit với ID null tại {Time}", DateTime.Now);
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                _logger.LogWarning("Không tìm thấy sách với ID {Id} tại {Time}", id, DateTime.Now);
                return NotFound();
            }

            var role = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                _logger.LogWarning("Truy cập không được phép vào Edit tại {Time}", DateTime.Now);
                return RedirectToAction("Login", "Account");
            }

            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "Fullname", book.AuthorId);
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CateName", book.CateId);
            return View(book);
        }

        // Xử lý chỉnh sửa sách khi form được submit
        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,AuthorId,Publisher,YearPublished,CateId,Quantity,Description,Status,ImagePath")] Book book, IFormFile ImageFile)
        {
            if (id != book.BookId)
            {
                _logger.LogWarning("ID không khớp khi chỉnh sửa sách tại {Time}", DateTime.Now);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null)
                    {
                        var filePath = Path.Combine("wwwroot/images/books", ImageFile.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                        book.ImagePath = $"/images/books/{ImageFile.FileName}";
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Cập nhật sách thành công với ID {Id} tại {Time}", book.BookId, DateTime.Now);
                    TempData["Message"] = "Cập nhật sách thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Xung đột khi cập nhật sách ID {Id} tại {Time}", id, DateTime.Now);
                    if (!BookExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "Fullname", book.AuthorId);
            ViewData["CateId"] = new SelectList(_context.Categories, "CateId", "CateName", book.CateId);
            return View(book);
        }

        // Hiển thị form xóa sách
        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Yêu cầu Delete với ID null tại {Time}", DateTime.Now);
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Cate)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                _logger.LogWarning("Không tìm thấy sách với ID {Id} tại {Time}", id, DateTime.Now);
                return NotFound();
            }

            var role = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                _logger.LogWarning("Truy cập không được phép vào Delete tại {Time}", DateTime.Now);
                return RedirectToAction("Login", "Account");
            }

            return View(book);
        }

        // Xử lý xóa sách khi xác nhận
        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                try
                {
                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Xóa sách thành công với ID {Id} tại {Time}", id, DateTime.Now);
                    TempData["Message"] = "Xóa sách thành công!";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi xóa sách ID {Id} tại {Time}", id, DateTime.Now);
                    return RedirectToAction("Error", "Home");
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // Kiểm tra sự tồn tại của sách
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}