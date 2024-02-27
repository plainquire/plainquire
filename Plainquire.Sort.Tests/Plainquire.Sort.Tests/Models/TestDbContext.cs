using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Plainquire.Sort.Tests.Models;

[ExcludeFromCodeCoverage]
public class TestDbContext<TValue> : DbContext
{
    private readonly bool _useSqlite;

    public TestDbContext(bool useSqlite = false)
        => _useSqlite = useSqlite;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (_useSqlite)
            optionsBuilder.UseSqlite($"Data Source={Guid.NewGuid()}.sqlite;");
        else
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<TestModel<TValue>>();

        modelBuilder
            .Entity<TestModel<TValue>>()
            .HasOne(x => x.NestedObject)
            .WithOne()
            .HasPrincipalKey<TestModel<TValue>>(x => x.Id);
    }

    public override void Dispose()
    {
        Database.EnsureDeleted();
        base.Dispose();
    }
}