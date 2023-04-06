using Microsoft.EntityFrameworkCore;
using Vox.Data.Extensions;

namespace Vox.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.UseEntityTypeConfiguration<AppDbContext>();
        modelBuilder.UseSnakeCaseNamingConvention();
    }
}
