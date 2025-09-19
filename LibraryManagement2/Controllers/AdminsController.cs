using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement2.Models;
using LibraryManagement2.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using NETCore.MailKit.Core;

namespace LibraryManagement2.Controllers
{
       [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập controller này
    public class AdminsController : Controller
    {
        private readonly LibraryDbContext _context; // Context để tương tác với cơ sở dữ liệu
        private readonly IEmailService _emailService; // Dịch vụ gửi email

        public AdminsController(LibraryDbContext context, IEmailService emailService)
        {
            _context = context; // Khởi tạo context từ DI
            _emailService = emailService; // Khởi tạo dịch vụ email từ DI
        }

        // Hiển thị trang chủ của Admin
        public IActionResult Index()
        {
            return View(); // Trả về view mặc định cho trang chủ
        }

        // Hiển thị form tạo tài khoản Sinh viên
        [HttpGet]
        public IActionResult CreateStudent()
        {
            return View(new RegisterStudentViewModel()); // Trả về view với model trống để nhập thông tin
        }

        // Xử lý tạo tài khoản Sinh viên khi form được submit
        [HttpPost]
        public async Task<IActionResult> CreateStudent(RegisterStudentViewModel model)
        {
            if (ModelState.IsValid && !_context.Students.Any(s => s.Email == model.Email))
            {
                // Kiểm tra dữ liệu hợp lệ và email chưa tồn tại
                var student = new Student
                {
                    Username = model.Username,
                    Password = model.Password, // Gán mật khẩu (cần hash thực tế)
                    Fullname = model.Fullname,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address,
                    Status = model.Status
                };

                _context.Students.Add(student); // Thêm sinh viên vào context
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

                var roleId = _context.Roles.FirstOrDefault(r => r.RoleName == "Student")?.RoleId;
                if (roleId.HasValue)
                {
                    _context.UserRoles.Add(new UserRole { RoleId = roleId.Value, StudentId = student.StudentId });
                    await _context.SaveChangesAsync(); // Gán vai trò Student
                }

                if (model.Status)
                    _emailService.SendEmail(model.Email, "Chào mừng", "Cảm ơn bạn đã tham gia hệ thống!"); // Gửi email chào mừng nếu active

                return RedirectToAction("Index"); // Chuyển hướng về trang chủ
            }
            return View(model); // Trả về view với lỗi nếu không hợp lệ
        }

        // Hiển thị form tạo tài khoản Thủ thư
        [HttpGet]
        public IActionResult CreateLibrarian()
        {
            return View(new RegisterLibrarianViewModel()); // Trả về view với model trống
        }

        // Xử lý tạo tài khoản Thủ thư khi form được submit
        [HttpPost]
        public async Task<IActionResult> CreateLibrarian(RegisterLibrarianViewModel model)
        {
            if (ModelState.IsValid && !_context.Librarians.Any(l => l.Email == model.Email))
            {
                // Kiểm tra dữ liệu hợp lệ và email chưa tồn tại
                var librarian = new Librarian
                {
                    Username = model.Username,
                    Password = model.Password, // Gán mật khẩu (cần hash thực tế)
                    Fullname = model.Fullname,
                    Email = model.Email,
                    HireDate = DateOnly.FromDateTime(model.HireDate),
                    Status = model.Status
                };

                _context.Librarians.Add(librarian); // Thêm thủ thư vào context
                await _context.SaveChangesAsync(); // Lưu thay đổi

                var roleId = _context.Roles.FirstOrDefault(r => r.RoleName == "Librarian")?.RoleId;
                if (roleId.HasValue)
                {
                    _context.UserRoles.Add(new UserRole { RoleId = roleId.Value, LibraId = librarian.LibraId });
                    await _context.SaveChangesAsync(); // Gán vai trò Librarian
                }

                if (model.Status)
                    _emailService.SendEmail(model.Email, "Chào mừng", "Cảm ơn bạn đã tham gia hệ thống!"); // Gửi email nếu active

                return RedirectToAction("Index"); // Chuyển hướng về trang chủ
            }
            return View(model); // Trả về view với lỗi nếu không hợp lệ
        }

        // Kiểm tra và gửi thông báo hạn trả/cảnh báo
        [HttpGet]
        public IActionResult CheckDueDates()
        {
            var currentDate = DateTime.Now;
            var dueDateReminder = currentDate.AddDays(2);

            // Convert DateTime to DateOnly for comparison
            var currentDateOnly = DateOnly.FromDateTime(currentDate);
            var dueDateReminderOnly = DateOnly.FromDateTime(dueDateReminder);

            var overdueBooks = _context.Borroweds
                .Where(b =>
                    b.DueDate.HasValue &&
                    b.DueDate.Value < currentDateOnly &&
                    b.ReturnDate == null &&
                    b.Student != null && b.Student.Status == true &&
                    b.Libra != null && b.Libra.Status == true
                )
                .Include(b => b.Student)
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .ToList();

            foreach (var borrow in overdueBooks)
            {
                if (borrow.Student != null && borrow.Student.Email != null)
                    _emailService.SendEmail(borrow.Student.Email, "Cảnh báo",
                        $"Sách '{borrow.Book?.Title}' đã quá hạn trả từ {borrow.DueDate:dd/MM/yyyy}.");
                if (borrow.Libra != null && borrow.Libra.Email != null)
                    _emailService.SendEmail(borrow.Libra.Email, "Cảnh báo",
                        $"Sách '{borrow.Book?.Title}' của {borrow.Student?.Fullname} đã quá hạn từ {borrow.DueDate:dd/MM/yyyy}.");
            }

            var reminderBooks = _context.Borroweds
                .Where(b =>
                    b.DueDate.HasValue &&
                    b.DueDate.Value == dueDateReminderOnly &&
                    b.ReturnDate == null &&
                    b.Student != null && b.Student.Status == true &&
                    b.Libra != null && b.Libra.Status == true
                )
                .Include(b => b.Student)
                .Include(b => b.Book)
                .Include(b => b.Libra)
                .ToList();

            foreach (var borrow in reminderBooks)
            {
                if (borrow.Student != null && borrow.Student.Email != null)
                    _emailService.SendEmail(borrow.Student.Email, "Nhắc nhở",
                        $"Sách '{borrow.Book?.Title}' đến hạn trả vào {borrow.DueDate:dd/MM/yyyy}.");
                if (borrow.Libra != null && borrow.Libra.Email != null)
                    _emailService.SendEmail(borrow.Libra.Email, "Nhắc nhở",
                        $"Sách '{borrow.Book?.Title}' của {borrow.Student?.Fullname} đến hạn vào {borrow.DueDate:dd/MM/yyyy}.");
            }

            return RedirectToAction("Index");
        }
    }
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"Gửi đến {to}: {subject} - {body}");
        }
    }
}