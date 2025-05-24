using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quangcao.Models
{
    public class ThanhVienDoiNgu
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required]
        [StringLength(100)]
        public string ViTri { get; set; }

        [Required]
        [StringLength(500)]
        public string TamHiet { get; set; }

        [Required]
        [StringLength(250)]
        public string AnhUrl { get; set; }

        [BindNever]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}
