using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;
using Quanlythuvien.ViewModels;
using System.Diagnostics;
using System.Linq;

namespace Quanlythuvien.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuanlythuvienDbContext _context;

        public HomeController(QuanlythuvienDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Kiểm tra null
        }

        public IActionResult Index(int categoryId = 0, string searchQuery = "")
        {
            // Lấy danh sách sách
            var booksQuery = _context.Books
                .Include(b => b.Categories)
                .ThenInclude(bc => bc.Categories)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .AsQueryable();

            if (categoryId != 0)
                booksQuery = booksQuery.Where(b => b.Categories.Any(c => c.CateId == categoryId));

            if (!string.IsNullOrWhiteSpace(searchQuery))
                booksQuery = booksQuery.Where(b => b.Title.Contains(searchQuery));

            var books = booksQuery
                .Select(b => new HomeBookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Authors = b.BookAuthors != null ? b.BookAuthors.Select(ba => ba.Author.AuthorName).ToList() : new List<string>(), // Điều hướng qua BookAuthors
                    PublisherName = b.Publisher != null ? b.Publisher.PublisherName : "",
                    YearPublished = b.YearPublished,
                    ImagePath = b.ImagePath ?? "",
                    CateName = b.Categories.Count > 0 ? string.Join(", ", b.Categories.Select(c => c.Categories.CateName)) : "Chưa phân loại"
                })
                .ToList();

            // Lấy top 5 sách được mượn nhiều
            var popularBooks = _context.Borroweds
                .GroupBy(bd => bd.BookId)
                .Select(g => new PopularBookViewModel
                {
                    BookId = (int)g.Key,
                    Title = g.Select(x => x.Book != null ? x.Book.Title : "").FirstOrDefault() ?? "",
                    TotalBorrows = g.Count()
                })
                .OrderByDescending(x => x.TotalBorrows)
                .Take(5)
                .ToList();

            ViewBag.PopularBooks = popularBooks;
            ViewBag.Books = books;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SearchQuery = searchQuery;

            return View();
        }

        public IActionResult Giới_thiệu() => View();

        public IActionResult Liên_hệ() => View();

        [HttpPost]
        public IActionResult Liên_hệ(string Name, string Email, string Message)
        {
            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Message))
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            ViewBag.Success = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ phản hồi sớm nhất!";
            return View();
        }

        public IActionResult Lỗi()
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