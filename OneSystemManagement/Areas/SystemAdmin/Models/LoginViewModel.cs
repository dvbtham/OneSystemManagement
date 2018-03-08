using System.ComponentModel.DataAnnotations;

namespace OneSystemManagement.Areas.SystemAdmin.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Bạn chưa nhập email")]
        [Display(Name = "Email")]
        [StringLength(300, ErrorMessage = "{0} chỉ nhập tối đa {1} ký tự")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ")]
        [Display(Name = "Mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [Display(Name = "Mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Bạn phải nhập độ dài mật khẩu ít nhất 6 ký tự")]
        public string NewPassword { get; set; }
    }
}
