using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using quangcao.Models;
using System.Reflection.Emit;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SanPham> SanPhams { get; set; }
    public DbSet<SanPhamYeuThich> SanPhamYeuThiches { get; set; }
    public DbSet<SanPhamTuongTu> SanPhamTuongTus { get; set; }
    public DbSet<DanhGia> DanhGias { get; set; }
    public DbSet<GioHang> GioHangs { get; set; }
    public DbSet<HoaDon> HoaDons { get; set; }
    public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    public DbSet<TinTuc> TinTucs { get; set; }
    public DbSet<LienHe> LienHes { get; set; }
    public DbSet<BaoGia> BaoGias { get; set; }
    public DbSet<AnhBiaTrang> AnhBiaTrangs { get; set; }
    public DbSet<GioiThieu> gioiThieus { get; set; }
    public DbSet<ThanhVienDoiNgu> thanhVienDoiNgus { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Cấu hình các thuộc tính decimal
        builder.Entity<BaoGia>().ToTable("baogia");

        builder.Entity<SanPham>()
            .Property(s => s.Gia)
            .HasColumnType("decimal(18,2)");

        builder.Entity<ChiTietHoaDon>()
            .Property(c => c.Gia)
            .HasColumnType("decimal(18,2)");

        builder.Entity<HoaDon>()
            .Property(h => h.TongTien)
            .HasColumnType("decimal(18,2)");

        builder.Entity<ChiTietHoaDon>()
            .Property(c => c.TongTien)
            .HasColumnType("decimal(18,2)");

        builder.Entity<GioHang>()
            .Property(g => g.Gia)
            .HasColumnType("decimal(18,2)");

        // Các cấu hình quan hệ khác của bạn
        // Cấu hình khóa chính cho bảng SanPhamYeuThich (nhiều-nhiều)
        builder.Entity<SanPhamYeuThich>()
            .HasKey(x => new { x.UserId, x.IdSanPham });

        // Cấu hình khóa chính cho bảng SanPhamTuongTu (nhiều-nhiều)
        builder.Entity<SanPhamTuongTu>()
            .HasKey(x => new { x.IdSanPham, x.IdSanPhamGoiY });

        // Cấu hình quan hệ giữa SanPham và SanPhamTuongTu (nhiều-một)
        builder.Entity<SanPhamTuongTu>()
            .HasOne(x => x.SanPham)
            .WithMany(x => x.TuongTuChinh) // SanPham có nhiều SanPhamTuongTu
            .HasForeignKey(x => x.IdSanPham)
            .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa sản phẩm nếu có sản phẩm liên quan

        // Cấu hình quan hệ giữa SanPhamGoiY và SanPhamTuongTu (nhiều-một)
        builder.Entity<SanPhamTuongTu>()
            .HasOne(x => x.SanPhamGoiY)
            .WithMany(x => x.TuongTuGoiY) // SanPhamGoiY có nhiều SanPhamTuongTu
            .HasForeignKey(x => x.IdSanPhamGoiY)
            .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa nếu có sản phẩm liên quan

        // Cấu hình mối quan hệ giữa ChiTietHoaDon và SanPham
        builder.Entity<ChiTietHoaDon>()
            .HasOne(x => x.SanPham)
            .WithMany(x => x.ChiTietHoaDons)
            .HasForeignKey(x => x.IdSanPham)
            .OnDelete(DeleteBehavior.Cascade); // Khi xóa sản phẩm thì xóa luôn ChiTietHoaDon

        // Cấu hình mối quan hệ giữa ChiTietHoaDon và HoaDon
        builder.Entity<ChiTietHoaDon>()
            .HasOne(x => x.HoaDon)
            .WithMany(x => x.ChiTietHoaDons)
            .HasForeignKey(x => x.IdHoaDon)
            .OnDelete(DeleteBehavior.Cascade); // Khi xóa hóa đơn thì xóa luôn ChiTietHoaDon
                                               // Cấu hình mối quan hệ giữa GioHang và SanPham
        builder.Entity<GioHang>()
            .HasOne(g => g.SanPham)
            .WithMany(sp => sp.GioHangs)
            .HasForeignKey(g => g.IdSanPham)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BaoGia>()
        .HasOne(b => b.User)  // BaoGia có một User (ApplicationUser)
        .WithMany(u => u.BaoGias) // ApplicationUser có nhiều BaoGia
        .HasForeignKey(b => b.UserId) // UserId là khóa ngoại
        .OnDelete(DeleteBehavior.Restrict); // Xử lý xóa dữ liệu
        builder.Entity<AnhBiaTrang>()
            .HasOne(a => a.User)
            .WithMany(u => u.AnhBiaTrangs)
            .HasForeignKey(a => a.UserId)
            .HasConstraintName("FK_AnhBiaTrangs_AspNetUsers_UserId")
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ThanhVienDoiNgu>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<GioiThieu>()
        .HasOne(g => g.User)
        .WithMany()
        .HasForeignKey(g => g.UserId)
        .OnDelete(DeleteBehavior.Restrict);

    }
}
