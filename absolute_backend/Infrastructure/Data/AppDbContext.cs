using Core.Entities;
using Infrastructure.Messaging.Sagas;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Title> Titles { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<TitleGenre> TitleGenres { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Credit> Credits { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Watchlist> Watchlists { get; set; }
    public DbSet<History> Histories { get; set; }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<OrderState> OrderStates => Set<OrderState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // auto decimal precision for all decimal properties(i forgor)
        /*foreach (var property in modelBuilder.Model
    .GetEntityTypes()
    .SelectMany(t => t.GetProperties())
    .Where(p => p.ClrType == typeof(decimal)))
{
    property.SetPrecision(18);
    property.SetScale(2);
} */
        modelBuilder.Entity<OrderState>()
       .Property(o => o.Amount)
       .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);
        // Composite keys
        modelBuilder.Entity<TitleGenre>()
            .HasKey(tg => new { tg.TitleId, tg.GenreId });

        modelBuilder.Entity<Credit>()
            .HasOne(c => c.Title)
            .WithMany(t => t.Credits)
            .HasForeignKey(c => c.TitleId);

        modelBuilder.Entity<Credit>()
            .HasOne(c => c.Person)
            .WithMany(p => p.Credits)
            .HasForeignKey(c => c.PersonId);

        base.OnModelCreating(modelBuilder);
    }
}
