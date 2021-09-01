using FS.FilterExpressionCreator.Filters;
using FS.FilterExpressionCreator.Interfaces;
using FS.FilterExpressionCreator.Models;
using FS.FilterExpressionCreator.Newtonsoft.Extensions;
using FS.FilterExpressionCreator.Tests.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FS.FilterExpressionCreator.Tests.Tests
{
    public static class TestMethods
    {
        public static List<TEntity> FilterDirectByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration, IPropertyFilterInterceptor interceptor)
        {
            var predicate = filter.CreateFilter(configuration, interceptor) ?? (x => true);
            return testItems.Where(predicate.Compile()).ToList();
        }

        public static List<TEntity> FilterNetCloneByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration, IPropertyFilterInterceptor interceptor)
        {
            var predicate = filter.Clone().CreateFilter(configuration, interceptor) ?? (x => true);
            return testItems.Where(predicate.Compile()).ToList();
        }

        public static List<TEntity> FilterNewtonCloneByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration, IPropertyFilterInterceptor interceptor)
        {
            var predicate = filter.NewtonsoftClone().CreateFilter(configuration, interceptor) ?? (x => true);
            return testItems.Where(predicate.Compile()).ToList();
        }

        public static List<TEntity> FilterDirectByEF<TEntity>(this ICollection<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration, IPropertyFilterInterceptor interceptor)
            where TEntity : class
        {
            var predicate = filter.CreateFilter(configuration, interceptor) ?? (x => true);
            return FilterByEF(testItems, predicate);
        }

        public static List<TEntity> FilterNetCloneByEF<TEntity>(this ICollection<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration, IPropertyFilterInterceptor interceptor)
            where TEntity : class
        {
            var predicate = filter.Clone().CreateFilter(configuration, interceptor) ?? (x => true);
            return FilterByEF(testItems, predicate);
        }

        public static List<TEntity> FilterNewtonCloneByEF<TEntity>(this ICollection<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration, IPropertyFilterInterceptor interceptor)
            where TEntity : class
        {
            var predicate = filter.NewtonsoftClone().CreateFilter(configuration, interceptor) ?? (x => true);
            return FilterByEF(testItems, predicate);
        }

        private static EntityFilter<TEntity> NewtonsoftClone<TEntity>(this EntityFilter<TEntity> filter)
        {
            var serializerSettings = new JsonSerializerSettings { Converters = JsonConverterExtensions.NewtonsoftConverters };
            var json = JsonConvert.SerializeObject(filter, serializerSettings);
            return JsonConvert.DeserializeObject<EntityFilter<TEntity>>(json, serializerSettings);
        }

        private static List<TEntity> FilterByEF<TEntity>(this ICollection<TEntity> testItems, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            using var dbContext = new TestDbContext<TEntity>();
            dbContext.Set<TEntity>().AddRange(testItems);
            dbContext.SaveChanges();

            return dbContext.Set<TEntity>().Where(predicate).ToList();
        }
    }
}
