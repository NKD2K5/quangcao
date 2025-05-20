using Microsoft.AspNetCore.Mvc;
using quangcao.Models;
using System.Linq;

namespace quangcao.ViewComponents
{
    public class BaoGiaListViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public BaoGiaListViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            // Lấy tất cả các báo giá từ cơ sở dữ liệu
            var danhSach = _context.BaoGias
                .OrderByDescending(x => x.NgayTao) // Sắp xếp theo ngày tạo, giảm dần
                .ToList(); // Lấy tất cả các báo giá

            return View(danhSach); // Trả về view với danh sách báo giá
        }
    }
}
