using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quangcao.Models;

namespace quangcao.Controllers
{
    [Authorize]
    public class BaoGiaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BaoGiaController(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: BaoGia
        public async Task<IActionResult> Index()
        {
            // Truy vấn báo giá, Include luôn User
            var list = await _context.BaoGias
                .Include(b => b.User)
                .ToListAsync();

            return View(list);
        }

        // GET: Tạo báo giá mới
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tạo báo giá mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BaoGia model)
        {
            // Lấy UserId từ người đăng nhập
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // Thêm lỗi ModelState nếu không tìm thấy User
                ModelState.AddModelError("", "Không tìm thấy thông tin người dùng.");
                return View(model);
            }

            // Không cần kiểm tra ModelState.IsValid trước khi gán các giá trị
            // => Gán giá trị cần thiết rồi mới kiểm tra hợp lệ
            model.IdBaoGia = Guid.NewGuid();
            model.UserId = userId;
            model.NgayTao = DateTime.Now;

            // Xóa User navigation để tránh EF bị lỗi tracking
            model.User = null;

            // Lúc này mới kiểm tra ModelState
            if (ModelState.IsValid)
            {
                try
                {
                    _context.BaoGias.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu báo giá: " + ex.Message);
                }
            }

            return View(model);
        }


        // GET: BaoGias/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Truy vấn báo giá và Include luôn User
            var baoGia = await _context.BaoGias
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.IdBaoGia == id);

            if (baoGia == null)
            {
                return NotFound();
            }

            return View(baoGia);
        }

        // POST: BaoGias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("IdBaoGia,TieuDe,NoiDung,NgayTao,UserId")] BaoGia baoGia)
        {
            if (id != baoGia.IdBaoGia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // EF Core tracking bằng Id
                    _context.Update(baoGia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BaoGiaExists(baoGia.IdBaoGia))
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

            return View(baoGia);
        }

        // GET: BaoGias/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include luôn User khi hiển thị
            var baoGia = await _context.BaoGias
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.IdBaoGia == id);

            if (baoGia == null)
            {
                return NotFound();
            }

            return View(baoGia);
        }

        // POST: BaoGias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var baoGia = await _context.BaoGias.FindAsync(id);
            if (baoGia != null)
            {
                _context.BaoGias.Remove(baoGia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BaoGiaExists(Guid id)
        {
            return _context.BaoGias.Any(e => e.IdBaoGia == id);
        }
    }
}
