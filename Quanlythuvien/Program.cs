using Microsoft.EntityFrameworkCore;
using Quanlythuvien.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;              
    options.Cookie.IsEssential = true;             
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();  
    logging.AddDebug();    
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<QuanlythuvienDbContext>(options =>
{
    try
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Chui k?t n?i DefaultConnection kh�ng ???c t�m th?y trong appsettings.json.");
        }
        options.UseSqlServer(connectionString);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"L?i c?u h�nh DbContext: {ex.Message}");
        throw; 
    }
});

var app = builder.Build();


if (app == null)
{
    throw new InvalidOperationException("ng??i d�ng kh�ng th? ???c x�y d?ng. Vui l�ng ki?m tra c?u h�nh d?ch v?.");
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
app.UseAuthorization();     


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();