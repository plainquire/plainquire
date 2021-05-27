using FilterExpressionCreator.Demo.Models;
using LinqToDB.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FilterExpressionCreator.Demo.Database
{
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

            LinqToDBForEFTools.Initialize();
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Freelancer>()
                .HasMany(x => x.Projects)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
