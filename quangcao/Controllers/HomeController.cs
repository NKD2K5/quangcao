using Microsoft.AspNetCore.Mvc;
using quangcao.Models;
using quangcao.Models.ViewModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _hostEnvironment;

    public HomeController(
        ILogger<HomeController> logger,
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _hostEnvironment = hostEnvironment;
    }

    // Helper method to get company introduction
    private GioiThieu GetCompanyIntroduction()
    {
        return _context.gioiThieus.FirstOrDefault();
    }

    // Helper method to check if user is admin
    private bool IsUserAdmin()
    {
        return User.Identity.IsAuthenticated;
    }

    // Helper method to get page cover image
    private AnhBiaTrang GetPageCoverImage(string tenTrang)
    {
        return _context.AnhBiaTrangs.FirstOrDefault(a => a.TenTrang == tenTrang);
    }

    // Helper method to get team members
    private List<ThanhVienDoiNgu> GetTeamMembers()
    {
        return _context.thanhVienDoiNgus.ToList();
    }

    // Helper method to prepare common ViewModel data
    private TrangChuViewModel PrepareCommonViewModel(string tenTrang)
    {
        var anhBia = GetPageCoverImage(tenTrang);
        var gioiThieu = GetCompanyIntroduction();
        var nhanVien = GetTeamMembers();

        return new TrangChuViewModel
        {
            AnhBia = anhBia,
            GioiThieu = gioiThieu,
            DanhSachNhanVien = nhanVien
        };
    }

    public async Task<IActionResult> Index()
    {
        // Đảm bảo lấy dữ liệu mới nhất từ database
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        // Lấy thông tin định danh trang
        string controller = ControllerContext.RouteData.Values["controller"]?.ToString();
        string action = ControllerContext.RouteData.Values["action"]?.ToString();
        string tenTrang = $"{controller}/{action}";

        // Truy vấn dữ liệu
        var latestNews = await _context.TinTucs
            .OrderByDescending(t => t.NgayDang)
            .Take(5)
            .ToListAsync();

        var viewModel = PrepareCommonViewModel(tenTrang);
        viewModel.LatestNews = latestNews;

        // Kiểm tra quyền
            ViewData["IsAdmin"] = IsUserAdmin();

        return View(viewModel);
    }


    public IActionResult Privacy()
    {
        // Đảm bảo lấy dữ liệu mới nhất từ database
        _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        string controller = ControllerContext.RouteData.Values["controller"]?.ToString();
        string action = ControllerContext.RouteData.Values["action"]?.ToString();
        string tenTrang = $"{controller}/{action}";

        var viewModel = PrepareCommonViewModel(tenTrang);

        // Kiểm tra quyền
        ViewData["IsAdmin"] = IsUserAdmin();

        return View(viewModel);
    }

    // Quản lý ảnh bìa
    [Authorize]
    public IActionResult CapNhatAnhBia()
    {
        try
        {
            var list = _context.AnhBiaTrangs.ToList();
            return View(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tải danh sách ảnh bìa");
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải danh sách ảnh bìa: " + ex.Message;
            return View(new List<AnhBiaTrang>());
        }
    }

    [Authorize]
    public IActionResult ThemAnhBia()
    {
        return View(new AnhBiaTrang());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ThemAnhBia(AnhBiaTrang model, IFormFile anhFile)
    {
        try
        {
            // Kiểm tra xem tên trang đã tồn tại chưa
            var existing = _context.AnhBiaTrangs.FirstOrDefault(a => a.TenTrang == model.TenTrang);
            if (existing != null)
            {
                TempData["ErrorMessage"] = $"Ảnh bìa cho trang '{model.TenTrang}' đã tồn tại. Vui lòng sử dụng chức năng cập nhật.";
                return View(model);
            }

            // Lấy user hiện tại
            var userId = _userManager.GetUserId(User);

            // Xử lý upload file
            if (anhFile != null && anhFile.Length > 0)
            {
                // Tạo tên file duy nhất
                var fileName = Path.GetFileNameWithoutExtension(anhFile.FileName);
                var extension = Path.GetExtension(anhFile.FileName);
                var uniqueFileName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{extension}";

                // Đường dẫn lưu file
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "covers");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await anhFile.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn ảnh
                model.DuongDanAnh = $"/images/covers/{uniqueFileName}";
                model.NgayCapNhat = DateTime.Now;
                model.UserId = userId;

                _context.AnhBiaTrangs.Add(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm ảnh bìa mới thành công!";
                return RedirectToAction("CapNhatAnhBia");
            }
            else
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file ảnh để tải lên!";
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thêm ảnh bìa mới");
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi thêm ảnh bìa mới: " + ex.Message;
            return View(model);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CapNhatAnhBia(int Id, string TenTrang, IFormFile anhFile)
    {
        try
        {
            // Lấy user hiện tại
            var userId = _userManager.GetUserId(User);

            // Tìm ảnh bìa hiện tại
            var existing = await _context.AnhBiaTrangs.FindAsync(Id);
            if (existing != null)
            {
                // Xử lý upload file
                if (anhFile != null && anhFile.Length > 0)
                {
                    // Lưu đường dẫn ảnh cũ để xóa sau khi cập nhật thành công
                    string oldImagePath = null;
                    if (!string.IsNullOrEmpty(existing.DuongDanAnh))
                    {
                        oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, existing.DuongDanAnh.TrimStart('/'));
                    }

                    // Tạo tên file duy nhất
                    var fileName = Path.GetFileNameWithoutExtension(anhFile.FileName);
                    var extension = Path.GetExtension(anhFile.FileName);
                    var uniqueFileName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{extension}";

                    // Đường dẫn lưu file
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "covers");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Lưu file
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await anhFile.CopyToAsync(fileStream);
                    }

                    // Cập nhật đường dẫn ảnh
                    existing.DuongDanAnh = $"/images/covers/{uniqueFileName}";
                    existing.NgayCapNhat = DateTime.Now;
                    existing.UserId = userId;

                    await _context.SaveChangesAsync();

                    // Xóa file ảnh cũ nếu có
                    if (oldImagePath != null && System.IO.File.Exists(oldImagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Không thể xóa file ảnh cũ: {0}", oldImagePath);
                        }
                    }

                    TempData["SuccessMessage"] = "Cập nhật ảnh bìa thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Vui lòng chọn file ảnh để cập nhật!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy ảnh bìa cần cập nhật!";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật ảnh bìa");
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật ảnh bìa: " + ex.Message;
        }

        return RedirectToAction("CapNhatAnhBia");
    }

    // Giới thiệu
    [Authorize]
    public IActionResult CapNhatGioiThieu()
    {
        var gt = GetCompanyIntroduction();
        if (gt == null)
        {
            // Tạo đối tượng mới nếu chưa có dữ liệu
            gt = new GioiThieu
            {
                TieuDe = "Giới Thiệu Về Công Ty Chúng Tôi",
                NoiDung = "<p>Đang cập nhật nội dung...</p>"
            };
        }
        return View(gt);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CapNhatGioiThieu(GioiThieu model, IFormFile anhFile)
    {
        try
        {
            // Lấy user hiện tại
            var userId = _userManager.GetUserId(User);

            // Xử lý upload ảnh giới thiệu
            if (anhFile != null && anhFile.Length > 0)
            {
                // Tạo tên file duy nhất
                var fileName = Path.GetFileNameWithoutExtension(anhFile.FileName);
                var extension = Path.GetExtension(anhFile.FileName);
                var uniqueFileName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{extension}";

                // Đường dẫn lưu file
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "about");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await anhFile.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn ảnh
                model.AnhGioiThieu = $"/images/about/{uniqueFileName}";
            }

            var existing = GetCompanyIntroduction();
            if (existing != null)
            {
                existing.TieuDe = model.TieuDe;
                existing.NoiDung = model.NoiDung;
                existing.UserId = userId;

                // Chỉ cập nhật ảnh nếu có ảnh mới
                if (!string.IsNullOrEmpty(model.AnhGioiThieu))
                {
                    existing.AnhGioiThieu = model.AnhGioiThieu;
                }
            }
            else
            {
                // Nếu chưa có dữ liệu giới thiệu, tạo mới
                model.UserId = userId;
                _context.gioiThieus.Add(model);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật nội dung giới thiệu thành công!";
            
            // Kiểm tra tham số redirectTo để quyết định chuyển hướng
            string redirectAction = HttpContext.Request.Query["redirectTo"].ToString();
            if (!string.IsNullOrEmpty(redirectAction) && redirectAction.Equals("Index", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index");
            }
            
            return RedirectToAction("Privacy");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật giới thiệu");
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật giới thiệu: " + ex.Message;
        }

        return RedirectToAction("Privacy");
    }

    // Đội ngũ nhân viên
    [Authorize]
    public IActionResult CapNhatNhanVien()
    {
        try
        {
            var list = _context.thanhVienDoiNgus.ToList();
            // Đảm bảo view đúng tên
            return View("CapNhatDoiNgu", list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tải danh sách nhân viên");
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải danh sách nhân viên: " + ex.Message;
            return View("CapNhatDoiNgu", new List<ThanhVienDoiNgu>());
        }
    }

    [Authorize]
    public IActionResult ThemNhanVien()
    {
        return View(new ThanhVienDoiNgu());
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ThemNhanVien(ThanhVienDoiNgu nv, IFormFile anhFile)
    {
        try
        {
            // Lấy user hiện tại
            var userId = _userManager.GetUserId(User);

            // Xử lý upload hình ảnh
            if (anhFile != null && anhFile.Length > 0)
            {
                // Tạo tên file duy nhất
                var fileName = Path.GetFileNameWithoutExtension(anhFile.FileName);
                var extension = Path.GetExtension(anhFile.FileName);
                var uniqueFileName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{extension}";

                // Đường dẫn lưu file
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "team");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await anhFile.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn ảnh
                nv.AnhUrl = $"/images/team/{uniqueFileName}";
            }

            // Đảm bảo các trường bắt buộc có giá trị
            if (string.IsNullOrEmpty(nv.HoTen))
            {
                nv.HoTen = "Chưa cập nhật";
            }

            if (string.IsNullOrEmpty(nv.ViTri))
            {
                nv.ViTri = "Nhân viên";
            }

            if (string.IsNullOrEmpty(nv.TamHiet))
            {
                nv.TamHiet = "Đang cập nhật...";
            }

            // Gán UserId
            nv.UserId = userId;

            // Thêm vào database
            _context.thanhVienDoiNgus.Add(nv);

            // Lưu thay đổi và bắt lỗi chi tiết
            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm thành viên mới thành công!";
            }
            catch (DbUpdateException dbEx)
            {
                var innerException = dbEx.InnerException;
                _logger.LogError(dbEx, "Lỗi Entity Framework khi thêm nhân viên");
                TempData["ErrorMessage"] = $"Lỗi khi lưu dữ liệu: {(innerException != null ? innerException.Message : dbEx.Message)}";

                // Ghi log chi tiết
                _logger.LogError(innerException, "Chi tiết lỗi khi thêm nhân viên");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thêm nhân viên");
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi thêm nhân viên: " + ex.Message;
        }

        return RedirectToAction("CapNhatNhanVien");
    }

    [Authorize]
    public async Task<IActionResult> SuaNhanVien(int id)
    {
        var nv = await _context.thanhVienDoiNgus.FindAsync(id);
        if (nv == null)
        {
            return NotFound();
        }
        return View(nv);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SuaNhanVien(ThanhVienDoiNgu nv, IFormFile anhFile)
    {
        try
        {
            // Lấy user hiện tại
            var userId = _userManager.GetUserId(User);

            var existing = await _context.thanhVienDoiNgus.FindAsync(nv.Id);
            if (existing == null)
            {
                return NotFound();
            }

            // Xử lý upload hình ảnh
            if (anhFile != null && anhFile.Length > 0)
            {
                // Tạo tên file duy nhất
                var fileName = Path.GetFileNameWithoutExtension(anhFile.FileName);
                var extension = Path.GetExtension(anhFile.FileName);
                var uniqueFileName = $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{extension}";

                // Đường dẫn lưu file
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "team");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await anhFile.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn ảnh
                existing.AnhUrl = $"/images/team/{uniqueFileName}";
            }

            // Cập nhật thông tin khác
            existing.HoTen = nv.HoTen;
            existing.ViTri = nv.ViTri;
            existing.TamHiet = nv.TamHiet;
            existing.UserId = userId;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật thông tin thành viên thành công!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật nhân viên");
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật nhân viên: " + ex.Message;
        }

        return RedirectToAction("CapNhatNhanVien");
    }

    [Authorize]
    public async Task<IActionResult> XoaNhanVien(int id)
    {
        try
        {
            var nv = await _context.thanhVienDoiNgus.FindAsync(id);
            if (nv != null)
            {
                _context.thanhVienDoiNgus.Remove(nv);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa thành viên thành công!";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa nhân viên");
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa nhân viên: " + ex.Message;
        }

        return RedirectToAction("CapNhatNhanVien");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
