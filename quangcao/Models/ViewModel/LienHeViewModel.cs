using System.ComponentModel.DataAnnotations;

namespace quangcao.Models.ViewModel
{
    public class LienHeViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }

        // Additional fields you might want to consider
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }
        public DateTime ThoiGian { get; set; }
    }
}