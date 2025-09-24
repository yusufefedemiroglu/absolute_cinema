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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
