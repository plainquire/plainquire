using Newtonsoft.Json;
using Plainquire.Filter.Newtonsoft;
using Plainquire.Filter.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Plainquire.Filter.Tests.Services;

public delegate List<TEntity> EntityFilterFunc<TEntity>(ICollection<TEntity> testItems, EntityFilter<TEntity> filter, IFilterInterceptor? interceptor = null);

[ExcludeFromCodeCoverage]
public static class EntityFilterFunctions
{
    public static IEnumerable<EntityFilterFunc<TEntity>> GetEntityFilterFunctions<TEntity>() where TEntity : class
        => [
            FilterDirectByLinq,
            FilterNetCloneByLinq,
            FilterNewtonCloneByLinq,
            FilterDirectByEF,
            FilterNetCloneByEF,
            FilterNewtonCloneByEF
        ];

    public static IEnumerable<object> GetEntityFilterFunctions(Type entityType)
        => (IEnumerable<object>)typeof(EntityFilterFunctions)
            .GetMethod(nameof(GetEntityFilterFunctions), BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly, [])!
            .MakeGenericMethod(entityType)
            .Invoke(null, [])!;

    private static List<TEntity> FilterDirectByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityFilter<TEntity> filter, IFilterInterceptor? interceptor)
    {
        var predicate = filter.CreateFilter(interceptor) ?? (x => true);
        return testItems.Where(predicate.Compile()).ToList();
    }

    private static List<TEntity> FilterNetCloneByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityFilter<TEntity> filter, IFilterInterceptor? interceptor)
    {
        var predicate = filter.Clone().CreateFilter(interceptor) ?? (x => true);
        return testItems.Where(predicate.Compile()).ToList();
    }

    private static List<TEntity> FilterNewtonCloneByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityFilter<TEntity> filter, IFilterInterceptor? interceptor)
    {
        var predicate = filter.NewtonsoftClone().CreateFilter(interceptor) ?? (x => true);
        return testItems.Where(predicate.Compile()).ToList();
    }

    private static List<TEntity> FilterDirectByEF<TEntity>(this ICollection<TEntity> testItems, EntityFilter<TEntity> filter, IFilterInterceptor? interceptor)
        where TEntity : class
    {
        var predicate = filter.CreateFilter(interceptor) ?? (x => true);
        return testItems.FilterByEF(predicate);
    }

    private static List<TEntity> FilterNetCloneByEF<TEntity>(this ICollection<TEntity> testItems, EntityFilter<TEntity> filter, IFilterInterceptor? interceptor)
        where TEntity : class
    {
        var predicate = filter.Clone().CreateFilter(interceptor) ?? (x => true);
        return testItems.FilterByEF(predicate);
    }

    private static List<TEntity> FilterNewtonCloneByEF<TEntity>(this ICollection<TEntity> testItems, EntityFilter<TEntity> filter, IFilterInterceptor? interceptor)
        where TEntity : class
    {
        var predicate = filter.NewtonsoftClone().CreateFilter(interceptor) ?? (x => true);
        return testItems.FilterByEF(predicate);
    }

    private static EntityFilter<TEntity> NewtonsoftClone<TEntity>(this EntityFilter<TEntity> filter)
    {
        var serializerSettings = new JsonSerializerSettings { Converters = JsonConverterExtensions.NewtonsoftConverters };
        var json = JsonConvert.SerializeObject(filter, serializerSettings);
        return JsonConvert.DeserializeObject<EntityFilter<TEntity>>(json, serializerSettings)!;
    }

    private static List<TEntity> FilterByEF<TEntity>(this IEnumerable<TEntity> testItems, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        using var dbContext = new TestDbContext<TEntity>();
        dbContext.Set<TEntity>().AddRange(testItems);
        dbContext.SaveChanges();

        return dbContext.Set<TEntity>().Where(predicate).ToList();
    }
}