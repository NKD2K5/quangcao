using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace quangcao.Models.ViewModel
{
    public class TinTucEditViewModel
    {
        public Guid IdTinTuc { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        public string TieuDe { get; set; }

        [Required(ErrorMessage = "Nội dung không được để trống")]
        public string NoiDung { get; set; }

        // Giữ lại Hình ảnh cũ để khi không cập nhật ảnh mới, bạn vẫn có thể giữ lại ảnh cũ
        public string? HinhAnhCu { get; set; }

        [Display(Name = "Hình ảnh mới")]
        public IFormFile? HinhAnhMoi { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày đăng")]
        public DateTime NgayDang { get; set; }

        [Display(Name = "Người đăng")]
        public Guid? UserId { get; set; }  // Sử dụng Guid thay vì string nếu UserId là Guid
    }
}
