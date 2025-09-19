using System.Diagnostics;
using LibraryManagement2.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; // Logger ?? ghi l?i thông tin và l?i

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // ??m b?o logger không null
        }

        // Hi?n th? trang ch? c?a ?ng d?ng
        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Truy c?p trang Index t?i {Time}", DateTime.Now); // Ghi log truy c?p
                return View(); // Tr? v? view m?c ??nh
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi truy c?p trang Index");
                return RedirectToAction("Error"); // Chuy?n h??ng ??n trang l?i n?u có exception
            }
        }

        // Hi?n th? trang chính sách b?o m?t
        public IActionResult Privacy()
        {
            try
            {
                _logger.LogInformation("Truy c?p trang Privacy t?i {Time}", DateTime.Now); // Ghi log truy c?p
                return View(); // Tr? v? view Privacy
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi truy c?p trang Privacy");
                return RedirectToAction("Error"); // Chuy?n h??ng ??n trang l?i n?u có exception
            }
        }

        // Hi?n th? trang l?i v?i thông tin chi ti?t
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogWarning("Truy c?p trang Error v?i RequestId: {RequestId}", Activity.Current?.Id ?? HttpContext.TraceIdentifier);
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, // L?y ID yêu c?u hi?n t?i
                Timestamp = DateTime.Now // Thêm th?i ?i?m x?y ra l?i
            });
        }
    }
}