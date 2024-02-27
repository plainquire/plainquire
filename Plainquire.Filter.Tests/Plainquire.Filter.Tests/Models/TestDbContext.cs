using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Plainquire.Filter.Tests.Models;

[ExcludeFromCodeCoverage]
public class TestDbContext<TEntity> : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        //_sqLiteDatabaseFile = $"{Guid.NewGuid()}";
        //optionsBuilder.UseSqlite($"Data Source={Guid.NewGuid()}.sqlite;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<TestModel<TEntity>>();

        modelBuilder
            .Entity<TestModel<TEntity>>()
            .HasOne(x => x.NestedObject)
            .WithOne()
            .HasPrincipalKey<TestModel<TEntity>>(x => x.Id)
            .HasForeignKey<TestModelNested>(x => x.ParentId);

        modelBuilder
            .Entity<TestModel<TEntity>>()
            .HasMany(x => x.NestedList)
            .WithOne()
            .HasPrincipalKey(x => x.Id)
            .HasForeignKey(x => x.ParentId);
    }

    public override void Dispose()
    {
        Database.EnsureDeleted();
        base.Dispose();
    }
}