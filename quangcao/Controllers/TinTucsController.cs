using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using quangcao.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Security.Claims;
using quangcao.Models.ViewModel;

namespace quangcao.Controllers
{
    public class TinTucsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;

        public TinTucsController(AppDbContext context, IWebHostEnvironment env, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        // Hiển thị danh sách tin tức
        public async Task<IActionResult> Index()
        {
            var tinTucs = await _context.TinTucs
                .Include(t => t.User)  // Nạp User vào để truy cập thông tin người đăng
                .ToListAsync();

            return View(tinTucs);
        }

        // Xem chi tiết tin tức
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            // Lấy thông tin tin tức cùng với người dùng liên quan
            var tinTuc = await _context.TinTucs
                .Include(t => t.User)  // Bao gồm thông tin người dùng
                .FirstOrDefaultAsync(m => m.IdTinTuc == id);

            if (tinTuc == null) return NotFound();

            // Tăng lượt xem
            if (tinTuc.ViewCount.HasValue)
                tinTuc.ViewCount += 1;
            else
                tinTuc.ViewCount = 1;

            await _context.SaveChangesAsync();

            // Lấy bài viết phổ biến (5 bài có lượt xem cao nhất)
            var popularPosts = await _context.TinTucs
                .Where(t => t.IdTinTuc != id) // Loại trừ bài hiện tại
                .OrderByDescending(t => t.ViewCount)
                .Take(5)
                .ToListAsync();
            ViewBag.PopularPosts = popularPosts;

            // Lấy bài viết liên quan (có thể dựa trên tags hoặc danh mục)
            // Ở đây tạm thời lấy 3 bài mới nhất khác bài hiện tại
            var relatedNews = await _context.TinTucs
                .Where(t => t.IdTinTuc != id)
                .OrderByDescending(t => t.NgayDang)
                .Take(3)
                .ToListAsync();
            ViewBag.RelatedNews = relatedNews;

            // Nếu có hệ thống tags, có thể lấy tags của bài viết
            // Giả sử có bảng TinTucTags liên kết với bảng Tags
            // var tags = await _context.TinTucTags
            //     .Where(tt => tt.TinTucId == id)
            //     .Include(tt => tt.Tag)
            //     .Select(tt => tt.Tag.Name)
            //     .ToListAsync();
            // ViewBag.Tags = tags;

            // Hoặc tạm thời để trống để view hiển thị tags mặc định
            ViewBag.Tags = new List<string>();

            return View(tinTuc);
        }


        // GET: Tạo tin tức mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tạo tin tức mới
        [HttpPost]
        public async Task<IActionResult> Create(TinTuc tinTuc, IFormFile HinhAnhFile)
        {
            try
            {
                // Lấy thông tin người dùng đang đăng nhập
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Kiểm tra và log dữ liệu
                Console.WriteLine($"TieuDe: {tinTuc.TieuDe}");
                Console.WriteLine($"NoiDung length: {tinTuc.NoiDung?.Length ?? 0}");
                Console.WriteLine($"HinhAnhFile: {HinhAnhFile?.FileName ?? "No file"}");
                Console.WriteLine($"UserId: {userId ?? "No user"}");

                if (!ModelState.IsValid)
                {
                    // Log lỗi ModelState
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                        }
                    }

                    return View("Create", tinTuc);
                }

                // Kiểm tra người dùng
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "Bạn cần đăng nhập để đăng tin tức");
                    return View("Create", tinTuc);
                }

                // Nếu có ảnh, tải lên
                if (HinhAnhFile != null && HinhAnhFile.Length > 0)
                {
                    // Kiểm tra kích thước file (ví dụ: giới hạn 5MB)
                    if (HinhAnhFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("HinhAnhFile", "Kích thước file không được vượt quá 5MB");
                        return View("Create", tinTuc);
                    }

                    // Kiểm tra định dạng file
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(HinhAnhFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("HinhAnhFile", "Chỉ chấp nhận file hình ảnh (jpg, jpeg, png, gif)");
                        return View("Create", tinTuc);
                    }

                    string folderPath = Path.Combine(_env.WebRootPath, "img", "tintuc");

                    // Đảm bảo thư mục tồn tại
                    if (!Directory.Exists(folderPath))
                    {
                        try
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"Không thể tạo thư mục lưu trữ: {ex.Message}");
                            return View("Create", tinTuc);
                        }
                    }

                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    string filePath = Path.Combine(folderPath, fileName);

                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await HinhAnhFile.CopyToAsync(stream);
                        }

                        tinTuc.HinhAnh = $"img/tintuc/{fileName}";
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Lỗi khi tải ảnh lên: " + ex.Message);
                        return View("Create", tinTuc);
                    }
                }

                // Gán UserId của người dùng đang đăng nhập
                tinTuc.UserId = userId;

                // Không cần tạo GUID mới vì model đã tự tạo
                // tinTuc.IdTinTuc = Guid.NewGuid();

                tinTuc.NgayDang = DateTime.Now;

                try
                {
                    _context.Add(tinTuc);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    var innerException = dbEx.InnerException;
                    string errorMessage = innerException != null ? innerException.Message : dbEx.Message;

                    // Log lỗi chi tiết
                    Console.WriteLine($"DbUpdateException: {dbEx.ToString()}");

                    ModelState.AddModelError("", $"Lỗi khi lưu tin tức: {errorMessage}");
                    return View("Create", tinTuc);
                }
                catch (Exception ex)
                {
                    var innerException = ex.InnerException;
                    string errorMessage = innerException != null ? innerException.Message : ex.Message;

                    // Log lỗi chi tiết
                    Console.WriteLine($"Exception: {ex.ToString()}");

                    ModelState.AddModelError("", $"Lỗi khi lưu tin tức: {errorMessage}");
                    return View("Create", tinTuc);
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi tổng quát
                Console.WriteLine($"Unexpected error: {ex.ToString()}");
                ModelState.AddModelError("", $"Lỗi không xác định: {ex.Message}");
                return View("Create", tinTuc);
            }
        }

        // GET: Chỉnh sửa tin tức
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var tinTuc = await _context.TinTucs.FindAsync(id);
            if (tinTuc == null) return NotFound();

            // Chuyển đổi từ TinTuc sang TinTucEditViewModel
            var model = new TinTucEditViewModel
            {
                IdTinTuc = tinTuc.IdTinTuc,
                TieuDe = tinTuc.TieuDe,
                NoiDung = tinTuc.NoiDung,
                NgayDang = tinTuc.NgayDang,
                UserId = Guid.TryParse(tinTuc.UserId.ToString(), out Guid userId) ? userId : (Guid?)null,
                HinhAnhCu = tinTuc.HinhAnh
            };

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", tinTuc.UserId);
            return View(model); // Truyền model là TinTucEditViewModel vào view
        }


        // POST: Chỉnh sửa tin tức
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TinTucEditViewModel model)
        {
            if (id != model.IdTinTuc) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", model.UserId);
                return View(model); // ✅ Đảm bảo model là TinTucEditViewModel
            }

            var tinTuc = await _context.TinTucs.FindAsync(id);
            if (tinTuc == null) return NotFound();

            // Cập nhật thông tin tin tức từ ViewModel
            tinTuc.TieuDe = model.TieuDe;
            tinTuc.NoiDung = model.NoiDung;
            tinTuc.NgayDang = model.NgayDang;
            if (model.UserId.HasValue)
            {
                tinTuc.UserId = model.UserId.Value.ToString();
            }
            else
            {
                tinTuc.UserId = null;
            }

            if (model.HinhAnhMoi != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "img", "tintuc");
                Directory.CreateDirectory(uploadsFolder); // Đảm bảo thư mục tồn tại

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.HinhAnhMoi.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.HinhAnhMoi.CopyToAsync(stream);
                }

                tinTuc.HinhAnh = "img/tintuc/" + fileName;
            }

            try
            {
                _context.Update(tinTuc);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", model.UserId);
                return View(model); // ✅ Đảm bảo đúng kiểu ViewModel
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tinTuc = await _context.TinTucs.FindAsync(id);
            if (tinTuc != null)
            {
                _context.TinTucs.Remove(tinTuc);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // Redirect lại về danh sách
        }

        private bool TinTucExists(Guid id)
        {
            return _context.TinTucs.Any(e => e.IdTinTuc == id);
        }
    }
}
