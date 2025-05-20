using System.ComponentModel.DataAnnotations;

namespace quangcao.Models
{
    public class LienHe
    {
        [Key]
        public Guid IdLienHe { get; set; } = Guid.NewGuid();

        [EmailAddress]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        public string? HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tiêu đề không được để trống.")]
        public string? TieuDe { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống.")]
        public string? NoiDung { get; set; }

        public DateTime ThoiGian { get; set; } = DateTime.Now;
    }
}
