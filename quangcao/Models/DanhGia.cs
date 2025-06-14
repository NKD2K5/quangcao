using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace quangcao.Models
{
    public class DanhGia
    {
        [Key]
        public Guid IdDanhGia { get; set; }

        [Required(ErrorMessage = "ID sản phẩm không được để trống")]
        public Guid IdSanPham { get; set; }

        [ForeignKey("IdSanPham")]
        public virtual SanPham SanPham { get; set; }

        [Required(ErrorMessage = "Tên người dùng không được để trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên người dùng phải từ 2-100 ký tự")]
        [Display(Name = "Tên người dùng")]
        public string TenNguoiDung { get; set; }

        [Required(ErrorMessage = "Số sao đánh giá không được để trống")]
        [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5")]
        [Display(Name = "Số sao")]
        public int SoSao { get; set; }

        [Required(ErrorMessage = "Nội dung đánh giá không được để trống")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Nội dung đánh giá phải từ 10-1000 ký tự")]
        [Display(Name = "Nội dung đánh giá")]
        public string BinhLuan { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [Required(ErrorMessage = "Ngày đánh giá không được để trống")]
        [Display(Name = "Ngày đánh giá")]
        [DataType(DataType.DateTime)]
        public DateTime NgayDanhGia { get; set; }

        [Display(Name = "Lượt hữu ích")]
        [Range(0, int.MaxValue, ErrorMessage = "Lượt hữu ích không được âm")]
        public int? LuotHuuIch { get; set; }

        [Display(Name = "Đã báo cáo")]
        public bool DaBaoCao { get; set; }
    }
}
