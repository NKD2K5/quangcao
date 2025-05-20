using quangcao.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 namespace quangcao.Models
{
    public class ChiTietHoaDon
    {
        [Key]
        public Guid IdChiTietHoaDon { get; set; } = Guid.NewGuid();

        [Required]
        public Guid IdSanPham { get; set; }

        [Required]
        public Guid IdHoaDon { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenSanPham { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
        public decimal Gia { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền phải >= 0")]
        public decimal TongTien { get; set; }

        [ForeignKey("IdSanPham")]
        public virtual SanPham SanPham { get; set; }

        [ForeignKey("IdHoaDon")]
        public virtual HoaDon HoaDon { get; set; }
    }
}
