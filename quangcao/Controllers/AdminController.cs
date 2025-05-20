using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace quangcao.Controllers
{
    [Authorize(Roles = "Admin")] // Chỉ cho phép Admin truy cập
    public class AdminController : Controller
    {
        // Trang Dashboard - Thông báo tổng quan
        public IActionResult Dashboard()
        {
            // Có thể truyền dữ liệu báo cáo tại đây
            return View();
        }

        // Quản lý sản phẩm
        public IActionResult ManageProducts()
        {
            // Lấy danh sách sản phẩm từ DB (giả lập)
            // var products = _productService.GetAll();
            return View(/*products*/);
        }

        // Quản lý tin tức
        public IActionResult ManageOrders()
        {
            // Đổi tên cho đúng ý nghĩa nếu cần (News instead of Orders?)
            // var newsList = _newsService.GetAll();
            return View(/*newsList*/);
        }

        // Quản lý liên hệ
        public IActionResult ManageUsers()
        {
            // var contacts = _contactService.GetAll();
            return View(/*contacts*/);
        }
    }
}
