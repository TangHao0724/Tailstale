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
    // �]�wSession�W��12
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
