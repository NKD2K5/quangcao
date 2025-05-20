using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quangcao.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace quangcao.Controllers
{
    public class SearchController : Controller
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string keyword, int page = 1)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return View(new SearchViewModel { Keyword = "" });
            }

            // Số lượng kết quả mỗi trang
            int pageSize = 9;

            // Tìm kiếm tin tức
            var newsQuery = _context.TinTucs
                .Where(t => t.TieuDe.Contains(keyword) ||
                           (t.NoiDung != null && t.NoiDung.Contains(keyword)))
                .OrderByDescending(t => t.NgayDang);

            var news = await newsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Tìm kiếm sản phẩm
            var productsQuery = _context.SanPhams
                .Where(p => p.TenSanPham.Contains(keyword) ||
                           (p.MoTa != null && p.MoTa.Contains(keyword)))
                .OrderByDescending(p => p.NgayTao);

            var products = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Đếm tổng số kết quả
            int totalNewsCount = await newsQuery.CountAsync();
            int totalProductsCount = await productsQuery.CountAsync();
            int totalCount = totalNewsCount + totalProductsCount;

            // Tính toán phân trang
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var viewModel = new SearchViewModel
            {
                Keyword = keyword,
                News = news,
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalResults = totalCount,
                TotalNewsCount = totalNewsCount,
                TotalProductsCount = totalProductsCount
            };

            return View(viewModel);
        }
    }
}
