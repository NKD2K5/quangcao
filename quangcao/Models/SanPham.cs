using System.ComponentModel.DataAnnotations;
namespace quangcao.Models
{ 
    public class SanPham
    {
        [Key]
        public Guid IdSanPham { get; set; } = Guid.NewGuid();
        public string TenSanPham { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Gia { get; set; }

        [Range(0, int.MaxValue)]
        public int? SoLuongDaBan { get; set; }

        public string? MoTa { get; set; }
        public string? ChiTiet { get; set; }

        public string? HinhAnh { get; set; }
        public DateTime NgayTao { get; set; }
        public string? TheLoai { get; set; }
        public bool IsRecentlyViewed { get; set; } = false;


        public ICollection<SanPhamYeuThich> YeuThichs { get; set; } = new List<SanPhamYeuThich>();
        public ICollection<SanPhamTuongTu> TuongTuChinh { get; set; } = new List<SanPhamTuongTu>();
        public ICollection<SanPhamTuongTu> TuongTuGoiY { get; set; } = new List<SanPhamTuongTu>();
        public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
        public ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();
        public ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}

