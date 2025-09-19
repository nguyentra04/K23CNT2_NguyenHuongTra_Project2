using System.ComponentModel.DataAnnotations;

namespace LibraryManagement2.Models
{
    public class RegisterLibrarianViewModel
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
        [StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Ngày tuyển dụng là bắt buộc.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày tuyển dụng")]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [Display(Name = "Trạng thái")]
        public bool Status { get; set; } = true; // Mặc định là 1 (active)

        
    }
}