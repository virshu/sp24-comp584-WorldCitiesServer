using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WorldCitiesModel;

public class WorldCitiesContext : IdentityDbContext<WorldCitiesUser>
{
    public WorldCitiesContext()
    {
    }

    public WorldCitiesContext(DbContextOptions<WorldCitiesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<City> Cities { get; set; } = null!;

    public virtual DbSet<Country> Countries { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        IConfigurationBuilder builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false);
        IConfigurationRoot configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // https://stackoverflow.com/a/45785019/184509
        base.OnModelCreating(builder);
        builder.Entity<City>(entity =>
        {
            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cities_Countries");
        });

        builder.Entity<Country>(entity =>
        {
            entity.Property(e => e.Iso2).IsFixedLength();
            entity.Property(e => e.Iso3).IsFixedLength();
        });
    }
}