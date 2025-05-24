using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quangcao.Models
{
    public class GioiThieu
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được bỏ trống")]
        [StringLength(200)]
        public string TieuDe { get; set; }

        [Required(ErrorMessage = "Nội dung không được bỏ trống")]
        public string NoiDung { get; set; }

        [Required(ErrorMessage = "Ảnh giới thiệu là bắt buộc")]
        [StringLength(250)]
        public string AnhGioiThieu { get; set; }

        [BindNever]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}
