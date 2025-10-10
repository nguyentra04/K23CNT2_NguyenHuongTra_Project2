using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var connectionString = builder.Configuration.GetConnectionString("DbConnect");
builder.Services.AddDbContext<QuanlythuvienDbContext>(options =>
    options.UseSqlServer(connectionString));

// Cấu hình Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<QuanlythuvienDbContext>()
.AddDefaultTokenProviders();

// Cấu hình xác thực bằng cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LogoutPath = "/Account/Logout";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

var app = builder.Build();

// Khởi tạo dữ liệu
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<QuanlythuvienDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Áp dụng migration

    

    // Tạo vai trò nếu chưa có
    var roles = new[] { "Admin", "Librarian", "Student" };
    foreach (var role in roles)
    {
        aw roleManager.CreateAsync(new IdentityRole(role));
    }

    // Tạo tài khoản admin mặc định nếu chưa có
    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser == null)
    {
        var user = new IdentityUser { UserName = "admin", Email = "admin@gmail.com" };
        var result = await userManager.CreateAsync(user, "123456");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
            Console.WriteLine(">> Tạo tài khoản admin mặc định: admin / 123456");
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();