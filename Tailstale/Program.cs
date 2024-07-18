using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tailstale.Data;
using Tailstale.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<TailstaleContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Tailstale")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    // �]�wSession�W��
    options.Cookie.Name = "LoginSession";
    // ���Cookie�ܭ��n
    options.Cookie.IsEssential = true;
    // ���i�H��JS���oCookie
    options.Cookie.HttpOnly = true;
    // ���w�u���HTTPS�ǰe
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    // �]�w�O�ɮɶ�(����)
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddControllersWithViews(options =>//����L�o����m�B
{
    //options.Filters.Add<>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();
app.UseAuthorization();
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
