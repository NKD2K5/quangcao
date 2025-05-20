using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using quangcao.Models;
using quangcao.Models.ViewModel;
using System.Security.Claims;

namespace quangcao
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddTransient<IEmailService, GmailEmailService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            // Cấu hình DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            // Cấu hình Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Cấu hình Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddControllersWithViews();

            // Cấu hình Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "YourCookieName";
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                options.ExpireTimeSpan = TimeSpan.FromMinutes(2);
                options.SlidingExpiration = false;
                options.Cookie.Expiration = null;
                options.Cookie.MaxAge = null;
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = builder.Configuration.GetSection("GoogleKey:ClientId").Value;
                options.ClientSecret = builder.Configuration.GetSection("GoogleKey:ClientSecret").Value;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            });

            var app = builder.Build();

            // Middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();           // Session phải đặt trước Auth
            app.UseAuthentication();    // Bắt buộc trước Authorization
            app.UseAuthorization();

            // Tạo tài khoản admin mặc định nếu chưa có
            await CreateAdminAccount(app);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        // Phương thức tạo tài khoản Admin nếu chưa tồn tại
        public static async Task CreateAdminAccount(WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Kiểm tra xem role Admin đã tồn tại chưa, nếu chưa thì tạo mới
            var roleExists = await roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Tạo tài khoản Admin mặc định
            var adminEmail = "admin@yourdomain.com";  // Địa chỉ email của Admin
            var adminPassword = "AdminPassword123!";  // Mật khẩu cho tài khoản Admin
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Tạo tài khoản admin mới
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Gán role Admin cho tài khoản này
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Không thể tạo tài khoản admin. Lỗi: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
