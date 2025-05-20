using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quangcao.Models
{
    public class DanhGia
    {
        [Key]
        public Guid IdDanhGia { get; set; }

        public Guid IdSanPham { get; set; }

        [ForeignKey("IdSanPham")]
        public virtual SanPham SanPham { get; set; }

        public string TenNguoiDung { get; set; }

        public int SoSao { get; set; }

        public string BinhLuan { get; set; }

        public string? HinhAnh { get; set; }

        public DateTime NgayDanhGia { get; set; }

        public int? LuotHuuIch { get; set; }

        public bool DaBaoCao { get; set; }
    }
}
