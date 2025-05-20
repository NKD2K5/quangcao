using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace quangcao.Models
{
    public class TinTuc
    {
        [Key]
        public Guid IdTinTuc { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
        public string TieuDe { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [DataType(DataType.MultilineText)]
        [Column(TypeName = "nvarchar(max)")]  // Thay đổi từ ntext sang nvarchar(max)
        public string NoiDung { get; set; }

        public string? HinhAnh { get; set; }
        public int? ViewCount { get; set; }
        public DateTime NgayDang { get; set; } = DateTime.Now;

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [NotMapped]
        public IFormFile? HinhAnhFile { get; set; }
    }
}