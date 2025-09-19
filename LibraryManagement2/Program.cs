using LibraryManagement2.Models;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IEmailService, EmailService>();
// Add services to the container
builder.Services.AddControllersWithViews();
// C?u hình session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// C?u hình logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});
// ??ng ký IHttpContextAccessor ?? s? d?ng trong các controller
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
// ??ng ký IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();