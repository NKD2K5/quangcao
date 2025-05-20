using quangcao.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class SanPhamYeuThich
{
    [Key]
    [Column(Order = 0)]
    public string UserId { get; set; }

    [Key]
    [Column(Order = 1)]
    public Guid IdSanPham { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }

    [ForeignKey(nameof(IdSanPham))]
    public SanPham SanPham { get; set; }
}
