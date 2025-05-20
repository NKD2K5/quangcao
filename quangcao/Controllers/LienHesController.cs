using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quangcao.Models;
using quangcao.Models.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace quangcao.Controllers
{
    public class LienHesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public LienHesController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: LienHes
        public async Task<IActionResult> Index()
        {
            // Không còn bao gồm User nữa, chỉ lấy thông tin về LienHe
            var lienHes = await _context.LienHes.ToListAsync();
            return View(lienHes);
        }

        // GET: LienHes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lienHe = await _context.LienHes
                .FirstOrDefaultAsync(m => m.IdLienHe == id);
            if (lienHe == null)
            {
                return NotFound();
            }

            return View(lienHe);
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: LienHes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LienHeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tạo đối tượng LienHe để lưu vào DB
                var lienHe = new LienHe
                {
                    IdLienHe = Guid.NewGuid(),
                    Email = model.Email,
                    TieuDe = model.TieuDe,
                    NoiDung = model.NoiDung,
                    ThoiGian = DateTime.Now,
                    HoTen = model.HoTen,
                    SoDienThoai = model.SoDienThoai
                };

                // Lưu vào cơ sở dữ liệu
                _context.LienHes.Add(lienHe);
                await _context.SaveChangesAsync();

                // Gửi email thông báo cho admin
                string subject = "📩 Bạn có một yêu cầu mới từ khách hàng!";
                string body = $@"
                    <h3><strong>Thông báo quan trọng: Một khách hàng vừa gửi thông tin liên hệ đến bạn!</strong></h3>
                    <p><strong>🎯 Họ tên:</strong> {model.HoTen}</p>
                    <p><strong>📱 Số điện thoại:</strong> {model.SoDienThoai}</p>
                    <p><strong>📧 Email:</strong> {model.Email}</p>
                    <p><strong>✉️ Nội dung liên hệ:</strong> {model.NoiDung}</p>
                    <p><strong>🕒 Thời gian gửi:</strong> {DateTime.Now}</p>
                    <p><em>Hãy nhanh chóng phản hồi để chăm sóc khách hàng một cách tốt nhất!</em></p>
                    ";
                // Điền email admin của bạn vào đây
                string adminEmail = "maianhnguyen2820@gmail.com"; // Thay bằng email của bạn
                await _emailService.SendEmailAsync(adminEmail, subject, body);

                // Thông báo gửi thành công
                TempData["Success"] = "Gửi liên hệ thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Nếu dữ liệu không hợp lệ, trả lại view với model
            return View(model);
        }


        // GET: LienHes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lienHe = await _context.LienHes
                .FirstOrDefaultAsync(m => m.IdLienHe == id);
            if (lienHe == null)
            {
                return NotFound();
            }

            return View(lienHe);
        }

        // POST: LienHes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lienHe = await _context.LienHes.FindAsync(id);
            if (lienHe != null)
            {
                _context.LienHes.Remove(lienHe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LienHeExists(Guid id)
        {
            return _context.LienHes.Any(e => e.IdLienHe == id);
        }
    }
}
