using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tailstale.Data;
using Tailstale.Models;
using Tailstale.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var Fuen104Policy = "Fuen104Policy";
builder.Services.AddCors(options => {
    options.AddPolicy(name: Fuen104Policy, policy => {
        policy.WithOrigins("*").WithMethods("*").WithHeaders("*");
    });
});




builder.Services.AddSession(option =>
{
    option.Cookie.Name = "Tailstate.Session";
    option.IdleTimeout = TimeSpan.FromMinutes(20);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
    option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddDbContext<TailstaleContext>(options =>
    options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("Tailstale")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    // 設定Session名稱12
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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//var ecPaySettings = builder.Configuration.GetSection("ECPay");
//builder.Services.Configure<ECPaySettings>(ecPaySettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
app.UseSession();

app.UseAuthorization();
app.UseCors();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
