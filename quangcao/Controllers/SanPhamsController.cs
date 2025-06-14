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
            try
            {
                // Validate input
                if (IdSanPham == Guid.Empty)
                {
                    return RedirectToAction("Details", new { id = IdSanPham });
                }

                if (string.IsNullOrWhiteSpace(TenNguoiDung))
                {
                    return RedirectToAction("Details", new { id = IdSanPham });
                }

                if (SoSao < 1 || SoSao > 5)
                {
                    return RedirectToAction("Details", new { id = IdSanPham });
                }

                if (string.IsNullOrWhiteSpace(BinhLuan))
                {
                    return RedirectToAction("Details", new { id = IdSanPham });
                }

                // Validate files
                if (MediaFiles != null && MediaFiles.Any())
                {
                    if (MediaFiles.Count > 5)
                    {
                        return RedirectToAction("Details", new { id = IdSanPham });
                    }

                    foreach (var file in MediaFiles)
                    {
                        if (file.Length > 10 * 1024 * 1024) // 10MB
                        {
                            return RedirectToAction("Details", new { id = IdSanPham });
                        }

                        var extension = Path.GetExtension(file.FileName).ToLower();
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".webm", ".ogg" };
                        if (!allowedExtensions.Contains(extension))
                        {
                            return RedirectToAction("Details", new { id = IdSanPham });
                        }
                    }
                }

                // Create review
                var danhGia = new DanhGia
                {
                    IdDanhGia = Guid.NewGuid(),
                    IdSanPham = IdSanPham,
                    TenNguoiDung = TenNguoiDung,
                    SoSao = SoSao,
                    BinhLuan = BinhLuan,
                    NgayDanhGia = DateTime.Now,
                    LuotHuuIch = 0,
                    DaBaoCao = false
                };

                // Save files if any
                if (MediaFiles != null && MediaFiles.Any())
                {
                    var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "danhgia");
                    Directory.CreateDirectory(uploadsFolderPath);

                    List<string> filePaths = new List<string>();
                    foreach (var file in MediaFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                            var filePath = Path.Combine(uploadsFolderPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            filePaths.Add($"uploads/danhgia/{fileName}");
                        }
                    }

                    if (filePaths.Any())
                    {
                        danhGia.HinhAnh = string.Join(";", filePaths);
                    }
                }

                _context.DanhGias.Add(danhGia);
                await _context.SaveChangesAsync();

                // Add success message
                TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi thành công!";

                // Redirect back to Details page
                return RedirectToAction("Details", new { id = IdSanPham });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Details", new { id = IdSanPham });
            }
        }

        public async Task<IActionResult> Details(Guid id, int? soSao, int page = 1)
        {
            var product = _context.SanPhams.FirstOrDefault(p => p.IdSanPham == id);
            if (product == null) return NotFound();


            // Lấy 3 tin tức mới nhất
            var tinTucs = _context.TinTucs
                .OrderByDescending(t => t.NgayDang)
                .Take(3)
                .ToList();

            // Xử lý sản phẩm đã xem gần đây
            var recentlyViewed = HttpContext.Session.GetObjectFromJson<List<SanPham>>("RecentlyViewed") ?? new List<SanPham>();
            recentlyViewed.RemoveAll(p => p.IdSanPham == product.IdSanPham);
            recentlyViewed.Insert(0, product);
            if (recentlyViewed.Count > 6)
                recentlyViewed = recentlyViewed.Take(6).ToList();
            HttpContext.Session.SetObjectAsJson("RecentlyViewed", recentlyViewed);

            // Lấy toàn bộ đánh giá của sản phẩm để tính thống kê
            var tatCaDanhGias = await _context.DanhGias
                .Where(d => d.IdSanPham == product.IdSanPham)
                .ToListAsync(); // Sử dụng async

            // Lọc đánh giá theo số sao (nếu có) và phân trang
            var danhGiasQuery = _context.DanhGias
                .Where(d => d.IdSanPham == product.IdSanPham);

            if (soSao.HasValue)
            {
                danhGiasQuery = danhGiasQuery.Where(d => d.SoSao == soSao.Value);
            }

            danhGiasQuery = danhGiasQuery.OrderByDescending(d => d.NgayDanhGia);

            var pageSize = 3;
            var totalRatings = await danhGiasQuery.CountAsync(); // Sử dụng async
            var totalPages = (int)Math.Ceiling((double)totalRatings / pageSize);
            
            // Đảm bảo page không vượt quá totalPages, và hợp lệ nếu totalPages là 0
            page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

            List<DanhGia> danhGiasHienTai;
            if (totalRatings == 0)
            {
                danhGiasHienTai = new List<DanhGia>();
                ViewBag.TotalRatings = 0;
                ViewBag.TotalPages = 0; 
                ViewBag.CurrentPage = 1;
            }
            else
            {
                danhGiasHienTai = await danhGiasQuery // Sử dụng async
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(); // Sử dụng async

                ViewBag.TotalRatings = totalRatings;
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page;
                ViewBag.SoSao = soSao;
            }

            // Tạo ViewModel để truyền dữ liệu sang view
            var viewModel = new sanphamChiTietViewModel
            {
                SanPham = product,
                TinTucs = tinTucs,
                TatCaDanhGia = tatCaDanhGias,       // Gán TatCaDanhGia vào ViewModel
                DanhGiaHienTai = danhGiasHienTai  // Gán DanhGiaHienTai (đã phân trang) vào ViewModel
            };

            // Sản phẩm tương tự
            var sanPhamTuongTu = _context.SanPhams
                .Where(s => s.TheLoai == product.TheLoai && s.IdSanPham != product.IdSanPham)
                .Take(9)
                .ToList();
            ViewBag.relatedProducts = sanPhamTuongTu;

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
            try
            {
                var danhGia = await _context.DanhGias.FindAsync(idDanhGia);
                if (danhGia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đánh giá" });
                }

                // Kiểm tra đăng nhập
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, requireLogin = true });
                }

                danhGia.DaBaoCao = true;
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> XoaDanhGia(Guid idDanhGia)
        {
            try
            {
                // Kiểm tra đăng nhập và quyền
                if (!User.Identity.IsAuthenticated || !(User.IsInRole("Admin") || User.IsInRole("Moderator")))
                {
                    return Json(new { success = false, message = "Bạn không có quyền xóa bình luận này" });
                }

                var danhGia = await _context.DanhGias.FindAsync(idDanhGia);
                if (danhGia == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bình luận" });
                }

                // Lưu lại IdSanPham trước khi xóa bình luận
                var idSanPham = danhGia.IdSanPham;

                // Xóa các file media nếu có
                if (!string.IsNullOrEmpty(danhGia.HinhAnh))
                {
                    var filePaths = danhGia.HinhAnh.Split(';');
                    foreach (var filePath in filePaths)
                    {
                        if (!string.IsNullOrWhiteSpace(filePath))
                        {
                            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                        }
                    }
                }

                // Xóa bình luận từ database
                _context.DanhGias.Remove(danhGia);
                await _context.SaveChangesAsync();

                // Trả về thông tin cần thiết cho client
                var remainingReviews = await _context.DanhGias.Where(d => d.IdSanPham == idSanPham).CountAsync();
                var avgRating = await _context.DanhGias
                    .Where(d => d.IdSanPham == idSanPham)
                    .Select(d => (double)d.SoSao)
                    .DefaultIfEmpty(0)
                    .AverageAsync();

                return Json(new { 
                    success = true, 
                    remainingCount = remainingReviews,
                    averageRating = Math.Round(avgRating, 1)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
                var mediaPaths = new List<string>();
                var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "img/sanpham");

                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                foreach (var file in hinhAnhFiles)
                {
                    // Kiểm tra kích thước file dựa vào loại
                    if (file.ContentType.StartsWith("image/"))
                    {
                        if (file.Length > 5 * 1024 * 1024) // 5MB cho ảnh
                        {
                            ModelState.AddModelError("", "Kích thước ảnh không được vượt quá 5MB");
                            ViewBag.ExistingCategories = DefaultCategories;
                            return View(sanPham);
                        }
                    }
                    else if (file.ContentType.StartsWith("video/"))
                    {
                        if (file.Length > 50 * 1024 * 1024) // 50MB cho video
                        {
                            ModelState.AddModelError("", "Kích thước video không được vượt quá 50MB");
                            ViewBag.ExistingCategories = DefaultCategories;
                            return View(sanPham);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Chỉ chấp nhận tập tin ảnh (jpg, png, gif) hoặc video (mp4, webm)");
                        ViewBag.ExistingCategories = DefaultCategories;
                        return View(sanPham);
                    }

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    mediaPaths.Add($"img/sanpham/{fileName}");
                }

                sanPham.HinhAnh = string.Join(";", mediaPaths);
            }

            sanPham.NgayTao = DateTime.Now;
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: SanPhams/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();

            ViewBag.ExistingCategories = DefaultCategories;
            return View(sanPham);
        }

        // POST: SanPhams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            [Bind("IdSanPham,TenSanPham,Gia,SoLuongDaBan,MoTa,ChiTiet,NgayTao,TheLoai,HinhAnh")] SanPham sanPham,
            List<IFormFile> hinhAnhFiles,
            List<string> AnhCuGiuLai)
        {
            if (id != sanPham.IdSanPham) return NotFound();

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ExistingCategories = DefaultCategories;
                    return View(sanPham);
                }

                var existingProduct = await _context.SanPhams.FindAsync(id);
                if (existingProduct == null) return NotFound();

                try
                {
                    // Đảm bảo AnhCuGiuLai không null
                    AnhCuGiuLai = AnhCuGiuLai ?? new List<string>();

                    // Danh sách media cũ
                    var oldFiles = existingProduct.HinhAnh?.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

                    // Xác định và xóa file không còn được giữ lại
                    var filesToDelete = oldFiles.Where(f => !AnhCuGiuLai.Contains(f)).ToList();

                    foreach (var file in filesToDelete)
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, file.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldFilePath);
                                Console.WriteLine($"Đã xóa file thành công: {oldFilePath}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Lỗi khi xóa file {oldFilePath}: {ex.Message}");
                            }
                        }
                    }

                    // Thư mục lưu file mới
                    var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "sanpham");
                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                        Console.WriteLine($"Đã tạo thư mục: {uploadFolder}");
                    }

                    // Upload và thêm file mới
                    if (hinhAnhFiles != null && hinhAnhFiles.Any())
                    {
                        foreach (var file in hinhAnhFiles)
                        {
                            if (file.Length > 0)
                            {
                                Console.WriteLine($"Đang xử lý file: {file.FileName}, Loại: {file.ContentType}, Kích thước: {file.Length / 1024.0 / 1024.0:F2}MB");

                                // Kiểm tra kích thước file dựa vào loại
                                if (file.ContentType.StartsWith("image/"))
                                {
                                    if (file.Length > 5 * 1024 * 1024) // 5MB cho ảnh
                                    {
                                        ModelState.AddModelError("", "Kích thước ảnh không được vượt quá 5MB");
                                        ViewBag.ExistingCategories = DefaultCategories;
                                        return View(sanPham);
                                    }
                                }
                                else if (file.ContentType.StartsWith("video/"))
                                {
                                    if (file.Length > 50 * 1024 * 1024) // 50MB cho video
                                    {
                                        ModelState.AddModelError("", "Kích thước video không được vượt quá 50MB");
                                        ViewBag.ExistingCategories = DefaultCategories;
                                        return View(sanPham);
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Chỉ chấp nhận tập tin ảnh (jpg, png, gif) hoặc video (mp4, webm)");
                                    ViewBag.ExistingCategories = DefaultCategories;
                                    return View(sanPham);
                                }

                                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                                var fileName = $"{Guid.NewGuid()}{extension}";
                                var filePath = Path.Combine(uploadFolder, fileName);

                                try
                                {
                                    using (var stream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await file.CopyToAsync(stream);
                                    }
                                    var relativePath = $"img/sanpham/{fileName}";
                                    AnhCuGiuLai.Add(relativePath);
                                    Console.WriteLine($"Đã lưu file thành công: {relativePath}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Lỗi khi lưu file {filePath}: {ex.Message}");
                                    ModelState.AddModelError("", $"Lỗi khi lưu file {file.FileName}: {ex.Message}");
                                    ViewBag.ExistingCategories = DefaultCategories;
                                    return View(sanPham);
                                }
                            }
                        }
                    }

                    // Cập nhật thông tin sản phẩm
                    existingProduct.TenSanPham = sanPham.TenSanPham;
                    existingProduct.Gia = sanPham.Gia;
                    existingProduct.SoLuongDaBan = sanPham.SoLuongDaBan;
                    existingProduct.MoTa = sanPham.MoTa;
                    existingProduct.ChiTiet = sanPham.ChiTiet;
                    existingProduct.TheLoai = sanPham.TheLoai;
                    
                    // Đảm bảo không lưu chuỗi rỗng vào HinhAnh
                    if (AnhCuGiuLai.Any())
                    {
                        existingProduct.HinhAnh = string.Join(";", AnhCuGiuLai);
                        Console.WriteLine($"Danh sách media được lưu: {existingProduct.HinhAnh}");
                    }
                    else
                    {
                        existingProduct.HinhAnh = null;
                        Console.WriteLine("Không có media nào được lưu");
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi trong quá trình xử lý: {ex.Message}");
                    ModelState.AddModelError("", $"Lỗi khi lưu sản phẩm: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi ngoại: {ex.Message}");
                ModelState.AddModelError("", $"Lỗi: {ex.Message}");
            }

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
                // XÓA ảnh vật lý trong thư mục wwwroot nếu có
                if (!string.IsNullOrEmpty(sanPham.HinhAnh))
                {
                    var imagePaths = sanPham.HinhAnh
                        .Split(';', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var imagePath in imagePaths)
                    {
                        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }

                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



        private bool SanPhamExists(Guid id)
        {
            return _context.SanPhams.Any(e => e.IdSanPham == id);
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteDirect(Guid id)
        {
            try
            {
                // Kiểm tra đăng nhập và quyền
                if (!User.Identity.IsAuthenticated || !(User.IsInRole("Admin") || User.IsInRole("Moderator")))
                {
                    return Json(new { success = false, message = "Bạn không có quyền xóa sản phẩm này" });
                }
                
                var sanPham = await _context.SanPhams.FindAsync(id);
                if (sanPham == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
                }

                // Xóa ảnh vật lý trong thư mục wwwroot nếu có
                if (!string.IsNullOrEmpty(sanPham.HinhAnh))
                {
                    var imagePaths = sanPham.HinhAnh
                        .Split(';', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var imagePath in imagePaths)
                    {
                        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }

                // Xóa các đánh giá liên quan đến sản phẩm
                var danhGias = await _context.DanhGias.Where(d => d.IdSanPham == id).ToListAsync();
                foreach (var danhGia in danhGias)
                {
                    // Xóa các file media của đánh giá nếu có
                    if (!string.IsNullOrEmpty(danhGia.HinhAnh))
                    {
                        var filePaths = danhGia.HinhAnh.Split(';');
                        foreach (var filePath in filePaths)
                        {
                            if (!string.IsNullOrWhiteSpace(filePath))
                            {
                                var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
                                if (System.IO.File.Exists(fullPath))
                                {
                                    System.IO.File.Delete(fullPath);
                                }
                            }
                        }
                    }
                    
                    _context.DanhGias.Remove(danhGia);
                }

                // Xóa sản phẩm
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Sản phẩm đã được xóa thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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

