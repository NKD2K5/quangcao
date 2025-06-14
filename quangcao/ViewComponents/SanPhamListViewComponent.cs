using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace quangcao.ViewComponents
{
    public class SanPhamListViewComponent : ViewComponent
    {
        // A nested class to represent the data structure for the view
        public class SanPhamCategoryGroup
        {
            public string GroupName { get; set; }
            public List<string> Categories { get; set; }
        }

        public IViewComponentResult Invoke()
        {
            var categoryGroups = new List<SanPhamCategoryGroup>
            {
                new SanPhamCategoryGroup
                {
                    GroupName = "In Ấn Văn Phòng",
                    Categories = new List<string> { "Danh Thiếp", "Giấy Tiêu Đề", "Hóa Đơn Bán Lẻ", "In Catalogue", "In Chứng Chỉ-Giấy Khen", "In Kẹp File-Folder", "In Phong Bì-Envelope" }
                },
                new SanPhamCategoryGroup
                {
                    GroupName = "Ấn Phẩm Tiếp Thị",
                    Categories = new List<string> { "In Menu-Thực Đơn", "In Tờ Rơi-Tờ Gấp", "Phiếu Quà Tặng" }
                },
                new SanPhamCategoryGroup
                {
                    GroupName = "In Bao Bì & Nhãn Mác",
                    Categories = new List<string> { "In Hộp Giấy", "In Túi Giấy", "In Nhãn Mác Sản Phẩm", "In Tem Nhãn", "In Phiếu Bảo Hành", "In Decal" }
                },
                new SanPhamCategoryGroup
                {
                    GroupName = "Quà Tặng",
                    Categories = new List<string> { "Bao Lì Xì", "In Lịch Tết", "Sổ Bìa Da", "Sổ Lò Xo", "Sổ Note" }
                },
                new SanPhamCategoryGroup
                {
                    GroupName = "Ấn Phẩm Khác",
                    Categories = new List<string> { "Thiệp Chúc Mừng", "Thiệp Cưới", "Vở Học Sinh" }
                }
            };

            return View(categoryGroups);
        }
    }
}
