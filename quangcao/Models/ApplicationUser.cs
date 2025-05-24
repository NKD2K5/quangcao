using Microsoft.AspNetCore.Identity;
using quangcao.Models;
using System.Collections.Generic;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? Avatar { get; set; }
    public string? DiaChi { get; set; }
    public string? HoTen { get; set; }
    public DateTime? NgaySinh { get; set; }

    public ICollection<SanPhamYeuThich> SanPhamYeuThiches { get; set; } = new List<SanPhamYeuThich>();
    public ICollection<GioHang> GioHangs { get; set; }
    public ICollection<HoaDon> HoaDons { get; set; }
    public ICollection<TinTuc> TinTucs { get; set; }
    public ICollection<BaoGia> BaoGias { get; set; }

    // ✅ Thêm quan hệ với các bảng mới
    public ICollection<GioiThieu> GioiThieus { get; set; }
    public ICollection<ThanhVienDoiNgu> ThanhVienDoiNgus { get; set; }
    public ICollection<AnhBiaTrang> AnhBiaTrangs { get; set; }
}
