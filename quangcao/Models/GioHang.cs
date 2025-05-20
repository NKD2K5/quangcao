using quangcao.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace quangcao.Models
{

    public class GioHang
    {
        [Key]
        public Guid IdGioHang { get; set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; }

        [Required]
        [ForeignKey("SanPham")] // chỉ rõ khóa ngoại cho Entity Framework
        public Guid IdSanPham { get; set; }

        public string TenSanPham { get; set; }

        [Range(0, int.MaxValue)]
        public int SoLuong { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Gia { get; set; }

        public string HinhAnh { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; }
        public SanPham SanPham { get; set; }
    }
}
