﻿using Newtonsoft.Json;
using Plainquire.Page.Newtonsoft;
using Plainquire.Page.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plainquire.Page.Tests.Services;

public delegate List<TEntity> EntityPageFunction<TEntity>(IEnumerable<TEntity> items, EntityPage<TEntity> page, IPageInterceptor? interceptor = null);

public static class EntityPageFunctions
{
    public static IEnumerable<EntityPageFunction<TEntity>> GetEntityPageFunctions<TEntity>() where TEntity : class
        => [
            PageDirectByLinq,
            PageNetCloneByLinq,
            PageNewtonCloneByLinq,
            PageDirectByEF,
            PageNetCloneByEF,
            PageNewtonCloneByEF
        ];

    public static IEnumerable<object> GetEntityPageFunctions(Type entityType)
        => (IEnumerable<object>)typeof(EntityPageFunctions)
            .GetMethod(nameof(GetEntityPageFunctions), BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly, [])!
            .MakeGenericMethod(entityType)
            .Invoke(null, [])!;

    private static List<TEntity> PageDirectByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityPage<TEntity> sort, IPageInterceptor? interceptor)
        => testItems.Page(sort, interceptor).ToList();

    private static List<TEntity> PageNetCloneByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityPage<TEntity> sort, IPageInterceptor? interceptor)
        => testItems.Page(sort.Clone(), interceptor).ToList();

    private static List<TEntity> PageNewtonCloneByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityPage<TEntity> sort, IPageInterceptor? interceptor)
        => testItems.Page(sort.NewtonsoftClone(), interceptor).ToList();

    private static List<TEntity> PageDirectByEF<TEntity>(this IEnumerable<TEntity> testItems, EntityPage<TEntity> sort, IPageInterceptor? interceptor)
        where TEntity : class
        => testItems.PageByEF(sort, interceptor);

    private static List<TEntity> PageNetCloneByEF<TEntity>(this IEnumerable<TEntity> testItems, EntityPage<TEntity> sort, IPageInterceptor? interceptor)
        where TEntity : class
        => testItems.PageByEF(sort.Clone(), interceptor);

    private static List<TEntity> PageNewtonCloneByEF<TEntity>(this IEnumerable<TEntity> testItems, EntityPage<TEntity> sort, IPageInterceptor? interceptor)
        where TEntity : class
        => testItems.PageByEF(sort.NewtonsoftClone(), interceptor);

    private static EntityPage<TEntity> NewtonsoftClone<TEntity>(this EntityPage<TEntity> sort)
    {
        var serializerSettings = new JsonSerializerSettings { Converters = JsonConverterExtensions.NewtonsoftConverters };
        var json = JsonConvert.SerializeObject(sort, serializerSettings);
        return JsonConvert.DeserializeObject<EntityPage<TEntity>>(json, serializerSettings)!;
    }

    private static List<TEntity> PageByEF<TEntity>(this IEnumerable<TEntity> testItems, EntityPage<TEntity> sort, IPageInterceptor? interceptor)
        where TEntity : class
    {
        using var dbContext = new TestDbContext<TEntity>();
        dbContext.Set<TEntity>().AddRange(testItems);
        dbContext.SaveChanges();

        return dbContext.Set<TEntity>().Page(sort, interceptor).ToList();
    }
}