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
    // 設定Session名稱
    options.Cookie.Name = "LoginSession";
    // 表示Cookie很重要
    options.Cookie.IsEssential = true;
    // 不可以用JS取得Cookie
    options.Cookie.HttpOnly = true;
    // 限定只能用HTTPS傳送
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    // 設定逾時時間(分鐘)
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddControllersWithViews(options =>//全域過濾器放置處
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
