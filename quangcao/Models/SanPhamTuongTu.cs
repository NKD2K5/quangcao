using quangcao.Models;
using System.ComponentModel.DataAnnotations;

public class SanPhamTuongTu
{
    [Key]
    public Guid IdSanPham { get; set; }
    public Guid IdSanPhamGoiY { get; set; }

    public SanPham SanPham { get; set; }
    public SanPham SanPhamGoiY { get; set; }
}