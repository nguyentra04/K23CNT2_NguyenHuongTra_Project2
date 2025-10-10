using System.ComponentModel.DataAnnotations;
namespace Quanlythuvien.Models;
public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    public string Fullname { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng chọn vai trò.")]
    public int RoleId { get; set; }   
}
