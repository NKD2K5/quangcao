using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using quangcao.Models;
using quangcao.Models.ViewModel;

namespace quangcao.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger , AppDbContext appDbContext)
        {
            _logger = logger;
            _context = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LienHeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    // Nếu chưa đăng nhập
                    return Unauthorized();
                }

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

                _context.Add(lienHe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
