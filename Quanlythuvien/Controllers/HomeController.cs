using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Quanlythuvien.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public HomeController(QuanlythuvienDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserRole")))
            {
                return View(); // Hi?n th? trang không ??ng nh?p
            }

            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.RedirectController = GetRedirectController(HttpContext.Session.GetString("UserRole"));

            try
            {
                var books = await _context.Books
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .Include(b => b.Publisher)
                    .OrderByDescending(b => b.BookId) // S? d?ng BookId nh? proxy cho th? t? m?i
                    .Take(6)
                    .ToListAsync();
                ViewBag.Books = books;
            }
            catch (Exception ex)
            {
                // Ghi log l?i (s? d?ng ILogger n?u có)
                Console.WriteLine($"L?i t?i sách: {ex.Message}");
                ViewBag.ErrorMessage = "Không th? t?i danh sách sách. Vui lòng th? l?i sau.";
            }

            return View();
        }

        // GET: Home/Books
        public async Task<IActionResult> Books()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var books = await _context.Books
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .Include(b => b.Publisher)
                    .OrderByDescending(b => b.BookId) // S?p x?p theo BookId gi?m d?n
                    .Take(6)
                    .ToListAsync();
                return View(books);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"L?i t?i sách: {ex.Message}");
                ViewBag.ErrorMessage = "Không th? t?i danh sách sách. Vui lòng th? l?i sau.";
                return View(new List<Book>());
            }
        }

        // GET: Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Home/Contact
        public IActionResult Contact()
        {
            return View();
        }

        // GET: Home/Search
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index");
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var books = await _context.Books
                    .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                    .Include(b => b.Publisher)
                    .Where(b => b.Title.Contains(query) ||
                               b.BookAuthors.Any(ba => ba.Author.AuthorName.Contains(query)) ||
                               b.Publisher.PublisherName.Contains(query)) // Thêm tìm ki?m theo nhà xu?t b?n
                    .Take(10)
                    .ToListAsync();
                ViewBag.Books = books;
                ViewBag.SearchQuery = query; // Truy?n query ?? hi?n th? trên view
            }
            catch (Exception ex)
            {
                Console.WriteLine($"L?i tìm ki?m: {ex.Message}");
                ViewBag.ErrorMessage = "Không th? tìm ki?m sách. Vui lòng th? l?i sau.";
            }

            return View("Index"); // S? d?ng l?i view Index ?? hi?n th? k?t qu?
        }

        private string GetRedirectController(string role)
        {
            return role switch
            {
                "Admin" => "Admins",
                "Librarian" => "Librarians", // S?a l?i chính t? t? "Librarian" thành "Librarians"
                "Student" => "Students",     // Thêm "Students" thay vì "Student"
                _ => "Home"                  // M?c ??nh quay l?i Home n?u vai trò không h?p l?
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    // Model cho Error (n?u c?n)
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}


