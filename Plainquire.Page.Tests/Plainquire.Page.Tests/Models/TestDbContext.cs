﻿using Microsoft.EntityFrameworkCore;
using System;

namespace Plainquire.Page.Tests.Models;

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
    }

    public override void Dispose()
    {
        Database.EnsureDeleted();
        base.Dispose();
    }
}