using Microsoft.EntityFrameworkCore;
using Tailstale.Models;


namespace Tailstale.partial;

public partial class TailstaleContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfiguration Config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(Config.GetConnectionString("Tailstale"));
            
        }
    }

    public DbSet<Tailstale.Models.keeper> keeper { get; set; } = default!;
    public DbSet<Tailstale.Models.business> business { get; set; } = default!;
    public DbSet<Tailstale.Models.Booking> Booking { get; set; } = default!;
    public DbSet<Tailstale.Models.BookingDetail> BookingDetail { get; set; } = default!;
    public DbSet<Tailstale.Models.Room> Room { get; set; } = default!;
    public DbSet<Tailstale.Models.order_status> Order_Status { get; set; } = default!;

}

