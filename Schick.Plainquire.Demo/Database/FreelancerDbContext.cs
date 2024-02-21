using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Schick.Plainquire.Demo.Models;

namespace Schick.Plainquire.Demo.Database;

/// <summary>
/// DB context for freelancers
/// Implements <see cref="DbContext" />
/// </summary>
/// <seealso cref="DbContext" />
public class FreelancerDbContext : DbContext
{
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        const string sqLiteDatabaseFile = "Freelancer";
        var connectionString = new SqliteConnectionStringBuilder { DataSource = $"{sqLiteDatabaseFile}.sqlite" }.ConnectionString;
        optionsBuilder.UseSqlite(connectionString);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Freelancer>()
            .HasMany(x => x.Projects)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<Freelancer>()
            .OwnsOne(x => x.Address);
    }
}