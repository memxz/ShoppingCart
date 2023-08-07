using ASPSession.Security;
using ASPSession.DAO;
using ASPSession.DAO.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISecurity, PvtSecurity>();    //custom-made DIs

builder.Services.AddSingleton<ICartDB, CartDB>();
builder.Services.AddSingleton<IGalleryDB, GalleryDB>();
builder.Services.AddSingleton<IOrderHistoryDB, OrderHistoryDB>();
builder.Services.AddSingleton<ISecurityDB, SecurityDB>();
builder.Services.AddSingleton<IRatingDB, RatingDB>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllersWithViews();





builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); //logs out if user AFK too long 
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/UserAccess/AccessDenied");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Gallery}/{action=Main}/{id?}");

app.Run();
