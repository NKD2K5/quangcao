using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quangcao.Models
{
    public class AnhBiaTrang
    {
        public int Id { get; set; }

        [Required]
        public string TenTrang { get; set; }   // VD: "TrangChu", "GioiThieu", "LienHe"

        [Required]
        public string DuongDanAnh { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;
        [BindNever]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }

}
