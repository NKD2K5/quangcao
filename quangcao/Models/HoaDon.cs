using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quangcao.Models
{

    public class HoaDon
    {
        [Key]
        public Guid IdHoaDon { get; set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; }

        [Required]
        public string TenKhachHang { get; set; }

        [Required]
        public DateTime ThoiGianDat { get; set; } = DateTime.Now;

        public DateTime? ThoiGianKetThuc { get; set; }

        [Required]
        public string TrangThaiHoaDon { get; set; }

        [Range(0, int.MaxValue)]
        public decimal TongTien { get; set; }
        public string? DiaChi { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}
