using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace quangcao.Models
{
    public class BaoGia
    {
        [Key]
        public Guid IdBaoGia { get; set; }

        [Required]
        public string TieuDe { get; set; }

        public string NoiDung { get; set; }

        public DateTime NgayTao { get; set; }

        // UserId sẽ không bị model binding check khi submit form
        [BindNever]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}
