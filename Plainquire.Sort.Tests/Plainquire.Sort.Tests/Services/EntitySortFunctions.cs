using Newtonsoft.Json;
using Plainquire.Sort.Newtonsoft;
using Plainquire.Sort.Tests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Plainquire.Sort.Tests.Services;

public delegate List<TEntity> EntitySortFunction<TEntity>(IEnumerable<TEntity> items, EntitySort<TEntity> sort, ISortInterceptor? interceptor = null);

public static class EntitySortFunctions
{
    public static IEnumerable<EntitySortFunction<TEntity>> GetEntitySortFunctions<TEntity>() where TEntity : class
        => [
            SortDirectByLinq,
            SortNetCloneByLinq,
            SortNewtonCloneByLinq,
            SortDirectByEF,
            SortNetCloneByEF,
            SortNewtonCloneByEF
        ];

    public static IEnumerable<object> GetEntitySortFunctions(Type entityType)
        => (IEnumerable<object>)typeof(EntitySortFunctions)
            .GetMethod(nameof(GetEntitySortFunctions), BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly, [])!
            .MakeGenericMethod(entityType)
            .Invoke(null, [])!;

    private static List<TEntity> SortDirectByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntitySort<TEntity> sort, ISortInterceptor? interceptor)
        => testItems.OrderBy(sort, interceptor).ToList();

    private static List<TEntity> SortNetCloneByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntitySort<TEntity> sort, ISortInterceptor? interceptor)
        => testItems.OrderBy(sort.Clone(), interceptor).ToList();

    private static List<TEntity> SortNewtonCloneByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntitySort<TEntity> sort, ISortInterceptor? interceptor)
        => testItems.OrderBy(sort.NewtonsoftClone(), interceptor).ToList();

    private static List<TEntity> SortDirectByEF<TEntity>(this IEnumerable<TEntity> testItems, EntitySort<TEntity> sort, ISortInterceptor? interceptor)
        where TEntity : class
        => testItems.OrderByEF(sort, interceptor);

    private static List<TEntity> SortNetCloneByEF<TEntity>(this IEnumerable<TEntity> testItems, EntitySort<TEntity> sort, ISortInterceptor? interceptor)
        where TEntity : class
        => testItems.OrderByEF(sort.Clone(), interceptor);

    private static List<TEntity> SortNewtonCloneByEF<TEntity>(this IEnumerable<TEntity> testItems, EntitySort<TEntity> sort, ISortInterceptor? interceptor)
        where TEntity : class
        => testItems.OrderByEF(sort.NewtonsoftClone(), interceptor);

    private static EntitySort<TEntity> NewtonsoftClone<TEntity>(this EntitySort<TEntity> sort)
    {
        var serializerSettings = new JsonSerializerSettings { Converters = JsonConverterExtensions.NewtonsoftConverters };
        var json = JsonConvert.SerializeObject(sort, serializerSettings);
        return JsonConvert.DeserializeObject<EntitySort<TEntity>>(json, serializerSettings)!;
    }

    private static List<TEntity> OrderByEF<TEntity>(this IEnumerable<TEntity> testItems, EntitySort<TEntity> sort, ISortInterceptor? interceptor)
        where TEntity : class
    {
        using var dbContext = new TestDbContext<TEntity>();
        dbContext.Set<TEntity>().AddRange(testItems);
        dbContext.SaveChanges();

        return dbContext.Set<TEntity>().OrderBy(sort, interceptor).ToList();
    }
}