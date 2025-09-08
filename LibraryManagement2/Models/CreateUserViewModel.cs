using System.ComponentModel.DataAnnotations;

namespace LibraryManagement2.Models
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái và số.")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc.")]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } // "Librarian" hoặc "Student"

        // Trường cho Student (tùy chọn)
        [Display(Name = "Mã sinh viên")]
        public string StudentId { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string username { get; set; }

        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        // Trường cho Librarian (tùy chọn)
        [Display(Name = "Mã nhân viên")]
        public string LibraId { get; set; }

        [Display(Name = "Chi nhánh thư viện")]
        public string LibraryBranch { get; set; }

        [Display(Name = "Ngày tuyển dụng")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }
    }
}