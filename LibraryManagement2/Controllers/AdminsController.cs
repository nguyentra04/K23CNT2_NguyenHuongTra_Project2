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
                    HireDate = model.HireDate,
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
            var currentDate = DateTime.Now; // Lấy ngày giờ hiện tại (12/09/2025 08:52 PM +07)
            var dueDateReminder = currentDate.AddDays(2); // Ngày nhắc nhở (14/09/2025)
            var overdueDate = currentDate; // Ngày kiểm tra quá hạn

            var overdueBooks = _context.Borrowed
                .Where(b => b.DueDate < overdueDate && b.ReturnDate == null && b.Student.Status && b.Librarian.Status)
                .Include(b => b.Student)
                .Include(b => b.Book)
                .Include(b => b.Librarian)
                .ToList();

            foreach (var borrow in overdueBooks)
            {
                if (borrow.Student != null)
                    _emailService.SendEmail(borrow.Student.Email, "Cảnh báo",
                        $"Sách '{borrow.Book.Title}' đã quá hạn trả từ {borrow.DueDate:dd/MM/yyyy}."); // Gửi cảnh báo cho sinh viên
                if (borrow.Librarian != null)
                    _emailService.SendEmail(borrow.Librarian.Email, "Cảnh báo",
                        $"Sách '{borrow.Book.Title}' của {borrow.Student?.Fullname} đã quá hạn từ {borrow.DueDate:dd/MM/yyyy}."); // Gửi cảnh báo cho thủ thư
            }

            var reminderBooks = _context.Borrowed
                .Where(b => b.DueDate == dueDateReminder && b.ReturnDate == null && b.Student.Status && b.Librarian.Status)
                .Include(b => b.Student)
                .Include(b => b.Book)
                .Include(b => b.Librarian)
                .ToList();

            foreach (var borrow in reminderBooks)
            {
                if (borrow.Student != null)
                    _emailService.SendEmail(borrow.Student.Email, "Nhắc nhở",
                        $"Sách '{borrow.Book.Title}' đến hạn trả vào {borrow.DueDate:dd/MM/yyyy}."); // Gửi nhắc nhở cho sinh viên
                if (borrow.Librarian != null)
                    _emailService.SendEmail(borrow.Librarian.Email, "Nhắc nhở",
                        $"Sách '{borrow.Book.Title}' của {borrow.Student?.Fullname} đến hạn vào {borrow.DueDate:dd/MM/yyyy}."); // Gửi nhắc nhở cho thủ thư
            }

            return RedirectToAction("Index"); // Chuyển hướng về trang chủ sau khi xử lý
        }
    }
}