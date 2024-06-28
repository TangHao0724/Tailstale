using Microsoft.EntityFrameworkCore;
using Tailstale.Models;


namespace Tailstale.partial;

partial class TailstaleContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfiguration Config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(Config.GetConnectionString("Tailstale"));
        }
    }

    public DbSet<Tailstale.Models.keeper> keeper { get; set; } = default!;
    public DbSet<Tailstale.Models.business> business { get; set; } = default!;

    
}

