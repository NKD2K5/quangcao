using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quangcao.Models;

using quangcao.Models.ViewModel;

namespace quangcao.Controllers
{
    public class SanPhamsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProductService _productService;

        public static readonly List<string> DefaultCategories = new List<string>
        {
            "Danh Thiếp", "Giấy Tiêu Đề", "Hóa Đơn Bán Lẻ", "In Catalogue", "In Chứng Chỉ-Giấy Khen",
            "In Kẹp File-Folder", "In Phong Bì-Envelope", "In Menu-Thực Đơn", "In Tờ Rơi-Tờ Gấp",
            "Phiếu Quà Tặng", "In Hộp Giấy", "In Túi Giấy", "In Nhãn Mác Sản Phẩm", "In Tem Nhãn",
            "In Phiếu Bảo Hành", "In Decal", "Bao Lì Xì", "In Lịch Tết", "Sổ Bìa Da", "Sổ Lò Xo",
            "Sổ Note", "Thiệp Chúc Mừng", "Thiệp Cưới", "Vở Học Sinh"
        };

        public SanPhamsController(AppDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, IProductService productService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _productService = productService;
        }
        public async Task<IActionResult> Index(string currentCategory, string sort, int page = 1)
        {
            const int PageSize = 9; // hoặc gán giá trị theo ý bạn

            // Lấy danh sách sản phẩm ban đầu
            var productsQuery = _context.SanPhams.AsQueryable();

            // Lọc theo thể loại
            if (!string.IsNullOrEmpty(currentCategory))
            {
                productsQuery = productsQuery.Where(p => p.TheLoai == currentCategory);
            }

            // Sắp xếp
            switch (sort)
            {
                case "price-asc":
                    productsQuery = productsQuery.OrderBy(p => p.Gia);
                    break;
                case "price-desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Gia);
                    break;
                case "popular":
                default:
                    productsQuery = productsQuery.OrderByDescending(p => p.SoLuongDaBan);
                    break;
            }

            // Tổng số sản phẩm
            int totalItems = await productsQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            // Đảm bảo số trang hợp lệ
            page = Math.Clamp(page, 1, Math.Max(1, totalPages));

            // Lấy danh sách phân trang
            var sanPhams = await productsQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Lấy danh sách thể loại
            var categories = DefaultCategories
                .Select(cat => new CategoryModel
                {
                    TheLoai = cat,
                    SoLuong = _context.SanPhams.Count(p => p.TheLoai == cat)
                })
                .ToList();

            // Lấy sản phẩm đã xem gần đây
            var recentlyViewed = HttpContext.Session.GetObjectFromJson<List<SanPham>>("RecentlyViewed");

            // Gán dữ liệu cho ViewBag
            ViewBag.Categories = categories;
            ViewBag.CurrentCategory = currentCategory;
            ViewBag.CurrentSort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;
            ViewBag.RecentlyViewed = recentlyViewed;

            return View(sanPhams);
        }

        public async Task<IActionResult> GuiDanhGia(Guid IdSanPham, int SoSao, string BinhLuan, string TenNguoiDung, List<IFormFile> MediaFiles)
        {
            // Kiểm tra IdSanPham có hợp lệ không
            if (IdSanPham == Guid.Empty)
            {
                TempData["Error"] = "Id sản phẩm không hợp lệ.";
                return RedirectToAction("Details", new { id = IdSanPham });
            }

            // Tạo đối tượng danh gia
            var danhGia = new DanhGia
            {
                IdSanPham = IdSanPham,
                TenNguoiDung = TenNguoiDung, // Lưu tên người dùng nhập vào
                SoSao = SoSao,
                BinhLuan = BinhLuan,
                HinhAnh = null, // Sẽ cập nhật sau nếu có hình ảnh
                NgayDanhGia = DateTime.Now
            };

            _context.DanhGias.Add(danhGia);
            await _context.SaveChangesAsync();

            // Đường dẫn thư mục để lưu file
            var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "danhgia");

            // Kiểm tra nếu thư mục không tồn tại thì tạo mới
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            // Lưu các file media (ảnh/video)
            if (MediaFiles != null && MediaFiles.Any())
            {
                List<string> filePaths = new List<string>();
                foreach (var file in MediaFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        // Tạo tên file duy nhất để tránh trùng lặp
                        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                        var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        filePaths.Add($"uploads/danhgia/{uniqueFileName}");
                    }
                }

                // Cập nhật đường dẫn hình ảnh vào đánh giá
                if (filePaths.Any())
                {
                    danhGia.HinhAnh = string.Join(";", filePaths);
                    await _context.SaveChangesAsync();
                }
            }

            // Trả về JSON response cho AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            // Trả về trang chi tiết sản phẩm
            return RedirectToAction("Details", new { id = IdSanPham });
        }

        public IActionResult Details(Guid id, int? soSao, int page = 1)
        {
            var product = _context.SanPhams.FirstOrDefault(p => p.IdSanPham == id);
            if (product == null) return NotFound();

            // Lấy 3 tin tức mới nhất
            var tinTucs = _context.TinTucs
                .OrderByDescending(t => t.NgayDang)
                .Take(3)
                .ToList();

            // Tạo ViewModel để truyền dữ liệu sang view
            var viewModel = new sanphamChiTietViewModel
            {
                SanPham = product,
                TinTucs = tinTucs
            };

            // Xử lý sản phẩm đã xem gần đây
            var recentlyViewed = HttpContext.Session.GetObjectFromJson<List<SanPham>>("RecentlyViewed") ?? new List<SanPham>();
            recentlyViewed.RemoveAll(p => p.IdSanPham == product.IdSanPham);
            recentlyViewed.Insert(0, product);
            if (recentlyViewed.Count > 6)
                recentlyViewed = recentlyViewed.Take(6).ToList();
            HttpContext.Session.SetObjectAsJson("RecentlyViewed", recentlyViewed);

            // Lấy toàn bộ đánh giá của sản phẩm để tính thống kê
            var tatCaDanhGias = _context.DanhGias
                .Where(d => d.IdSanPham == product.IdSanPham)
                .ToList();
            ViewBag.TatCaDanhGias = tatCaDanhGias;

            // Lọc đánh giá theo số sao (nếu có)
            var danhGiasQuery = _context.DanhGias
                .Where(d => d.IdSanPham == product.IdSanPham);

            if (soSao.HasValue)
            {
                danhGiasQuery = danhGiasQuery.Where(d => d.SoSao == soSao.Value);
            }

            // Sắp xếp sau khi đã lọc xong
            danhGiasQuery = danhGiasQuery.OrderByDescending(d => d.NgayDanhGia);

            var pageSize = 3;
            var totalRatings = danhGiasQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalRatings / pageSize);
            
            // Đảm bảo page không vượt quá totalPages
            page = Math.Max(1, Math.Min(page, totalPages));

            if (totalRatings == 0)
            {
                ViewBag.DanhGias = new List<DanhGia>();
                ViewBag.TotalRatings = 0;
                ViewBag.TotalPages = 0;
                ViewBag.CurrentPage = 1;
            }
            else
            {
                var danhGias = danhGiasQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.DanhGias = danhGias;
                ViewBag.TotalRatings = totalRatings;
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page;
                ViewBag.SoSao = soSao;
            }

            // Sản phẩm tương tự
            var sanPhamTuongTu = _context.SanPhams
                .Where(s => s.TheLoai == product.TheLoai && s.IdSanPham != product.IdSanPham)
                .Take(4)
                .ToList();
            ViewBag.SanPhamTuongTu = sanPhamTuongTu;

            return View(viewModel);
        }

        // Thêm phương thức để xử lý đánh giá hữu ích
        [HttpPost]
        public async Task<IActionResult> DanhGiaHuuIch(Guid idDanhGia)
        {
            var danhGia = await _context.DanhGias.FindAsync(idDanhGia);
            if (danhGia == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đánh giá" });
            }

            // Tăng số lượt đánh giá hữu ích
            danhGia.LuotHuuIch = (danhGia.LuotHuuIch ?? 0) + 1;
            await _context.SaveChangesAsync();

            return Json(new { success = true, helpfulCount = danhGia.LuotHuuIch });
        }

        // Thêm phương thức để xử lý báo cáo đánh giá
        [HttpPost]
        public async Task<IActionResult> BaoCaoDanhGia(Guid idDanhGia)
        {
            var danhGia = await _context.DanhGias.FindAsync(idDanhGia);
            if (danhGia == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đánh giá" });
            }

            // Đánh dấu đánh giá đã bị báo cáo
            danhGia.DaBaoCao = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
        public IActionResult Create()
        {
            ViewBag.ExistingCategories = DefaultCategories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham, List<IFormFile> hinhAnhFiles, string NewCategory)
        {
            if (sanPham.TheLoai == "Thêm thể loại mới")
            {
                if (string.IsNullOrWhiteSpace(NewCategory))
                {
                    ModelState.AddModelError("NewCategory", "Vui lòng nhập thể loại mới.");
                }
                else
                {
                    sanPham.TheLoai = NewCategory;
                }
            }
            else
            {
                // Xóa lỗi cũ của NewCategory nếu có
                ModelState.Remove("NewCategory");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ExistingCategories = DefaultCategories;

                return View(sanPham);
            }

            if (!DefaultCategories.Contains(sanPham.TheLoai))
            {
                ModelState.AddModelError("TheLoai", "Thể loại không hợp lệ.");
            }

            if (hinhAnhFiles != null && hinhAnhFiles.Any())
            {
                var imagePaths = new List<string>();
                var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "img/sanpham");

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                foreach (var file in hinhAnhFiles)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imagePaths.Add($"img/sanpham/{fileName}");
                }

                sanPham.HinhAnh = string.Join(";", imagePaths);
            }

            sanPham.NgayTao = DateTime.Now;
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();
            ViewBag.ExistingCategories = DefaultCategories;
            return View(sanPham);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IdSanPham,TenSanPham,Gia,SoLuongDaBan,MoTa,HinhAnh,NgayTao,TheLoai")] SanPham sanPham, IFormFile imageFile)
        {
            if (id != sanPham.IdSanPham) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý upload hình ảnh mới nếu có
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(sanPham.HinhAnh))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, sanPham.HinhAnh.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Lưu ảnh mới
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                        var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "sanpham");
                        
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        var filePath = Path.Combine(folderPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        sanPham.HinhAnh = $"/img/sanpham/{fileName}";
                    }

                    // Cập nhật sản phẩm
                    var existingProduct = await _context.SanPhams.FindAsync(id);
                    if (existingProduct != null)
                    {
                        existingProduct.TenSanPham = sanPham.TenSanPham;
                        existingProduct.Gia = sanPham.Gia;
                        existingProduct.SoLuongDaBan = sanPham.SoLuongDaBan;
                        existingProduct.MoTa = sanPham.MoTa;
                        existingProduct.TheLoai = sanPham.TheLoai;
                        
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            existingProduct.HinhAnh = sanPham.HinhAnh;
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanPhamExists(sanPham.IdSanPham))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, load lại danh sách thể loại
            ViewBag.ExistingCategories = DefaultCategories;
                
            return View(sanPham);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams.FirstOrDefaultAsync(m => m.IdSanPham == id);
            if (sanPham == null) return NotFound();

            return View(sanPham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanPhamExists(Guid id)
        {
            return _context.SanPhams.Any(e => e.IdSanPham == id);
        }
        [HttpGet]
        public async Task<IActionResult> QuickView(Guid id)
        {
            var sanPham = await _context.SanPhams
                .Include(s => s.DanhGias)
                .FirstOrDefaultAsync(s => s.IdSanPham == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return PartialView("_QuickView", sanPham);
        }
    }
}
