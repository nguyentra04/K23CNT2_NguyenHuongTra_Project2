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
            try
            {
                var books = await _context.Books
                    .Include(b => b.Authors)
                    .ThenInclude(a => a) // Remove this line if Author does not have navigation properties to include
                    .Include(b => b.Publisher)
                    .OrderByDescending(b => b.BookId)
                    .Take(6)
                    .ToListAsync();
                ViewBag.Books = books;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"L?i t?i s�ch: {ex.Message}");
                ViewBag.ErrorMessage = "Kh�ng th? t?i danh s�ch s�ch.";
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
                    .Include(b => b.Authors)
                    .ThenInclude(a => a) // Remove this line if Author does not have navigation properties to include
                    .Include(b => b.Publisher)
                    .OrderByDescending(b => b.BookId)
                    .Take(6)
                    .ToListAsync();
                return View(books);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"L?i t?i s�ch: {ex.Message}");
                ViewBag.ErrorMessage = "Kh�ng th? t?i danh s�ch s�ch. Vui l�ng th? l?i sau.";
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
                    .Include(b => b.Authors)
                    .Include(b => b.Publisher)
                    .Where(b => b.Title.Contains(query) ||
                               b.Authors.Any(a => a.AuthorName.Contains(query)) ||
                               b.Publisher.PublisherName.Contains(query)) 
                    .Take(10)
                    .ToListAsync();
                ViewBag.Books = books;
                ViewBag.SearchQuery = query; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"L?i t�m ki?m: {ex.Message}");
                ViewBag.ErrorMessage = "Kh�ng th? t�m ki?m s�ch. Vui l�ng th? l?i sau.";
            }

            return View("Index"); // S? d?ng l?i view Index ?? hi?n th? k?t qu?
        }

        private string GetRedirectController(string role)
        {
            return role switch
            {
                "Admin" => "Admins",
                "Librarian" => "Librarians", // S?a l?i ch�nh t? t? "Librarian" th�nh "Librarians"
                "Student" => "Students",     // Th�m "Students" thay v� "Student"
                _ => "Home"                  // M?c ??nh quay l?i Home n?u vai tr� kh�ng h?p l?
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


