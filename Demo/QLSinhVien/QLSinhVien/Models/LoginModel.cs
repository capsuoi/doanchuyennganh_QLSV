using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QLSinhVien.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        public string Password { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        public string MatKhau { get; set; }
        [Required(ErrorMessage = "Mật khẩu mới không được bỏ trống")]
        public string MatKhauMoi { get; set; }
        [Required(ErrorMessage = "Mật khẩu nhắc lại không được bỏ trống")]
        public string NhacLaiMatKhau { get; set; }
    }
}