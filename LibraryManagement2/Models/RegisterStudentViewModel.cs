using System.ComponentModel.DataAnnotations;

namespace LibraryManagement2.Models
{
    public class RegisterStudentViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tên đăng nhập chỉ được chứa chữ cái và số.")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự.")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [StringLength(255, ErrorMessage = "Mật khẩu không được vượt quá 255 ký tự.")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Tên đầy đủ là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên đầy đủ không được vượt quá 100 ký tự.")]
        [Display(Name = "Họ và tên")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        [RegularExpression(@"^[\w-\.]+@student\.university\.com$", ErrorMessage = "Email phải thuộc domain @student.university.com.")]
        [StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có đúng 10 chữ số.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số điện thoại chỉ được chứa số.")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [StringLength(50, ErrorMessage = "Địa chỉ không được vượt quá 50 ký tự.")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [Display(Name = "Trạng thái")]
        public bool Status { get; set; } = true; // Mặc định là 1 (active)
    }
}