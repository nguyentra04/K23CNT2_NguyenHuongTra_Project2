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
             public async Task<IActionResult> Index(int? cateId)
        {
            var categories = await _context.Categories.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = cateId;

            var books = _context.Books.Include(b => b.Categories).AsQueryable();

            if (cateId.HasValue)
            {
                books = books.Where(b => b.Categories.Any(c => c.CateId == cateId.Value));
            }

            return View(await books.ToListAsync());
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
        [HttpPost]
        public IActionResult Contact(string Name, string Email, string Message)
        {
            ViewBag.Success = "C?m ?n b?n ?ã liên h?. Chúng tôi s? ph?n h?i s?m nh?t!";
            return View();
        }

        public IActionResult Introduce()
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
                Console.WriteLine($"L?i tìm ki?m: {ex.Message}");
                ViewBag.ErrorMessage = "Không th? tìm ki?m sách. Vui lòng th? l?i sau.";
            }

            return View("Index"); 
        }

        private string GetRedirectController(string role)
        {
            return role switch
            {
                "Admin" => "Admins",
                "Librarian" => "Librarians", 
                "Student" => "Students",     
                _ => "Home"                  
            };
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}


