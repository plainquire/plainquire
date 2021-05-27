using FilterExpressionCreator.Models;
using FilterExpressionCreator.Newtonsoft.Extensions;
using FilterExpressionCreator.Tests.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FilterExpressionCreator.Tests.Tests
{
    public static class TestMethods
    {
        public static List<TEntity> FilterDirectByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration)
        {
            var predicate = filter.CreateFilterPredicate(configuration) ?? (x => true);
            return testItems.Where(predicate.Compile()).ToList();
        }

        public static List<TEntity> FilterNetCloneByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration)
        {
            var predicate = filter.Clone().CreateFilterPredicate(configuration) ?? (x => true);
            return testItems.Where(predicate.Compile()).ToList();
        }

        public static List<TEntity> FilterNewtonCloneByLinq<TEntity>(this IEnumerable<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration)
        {
            var predicate = filter.NewtonsoftClone().CreateFilterPredicate(configuration) ?? (x => true);
            return testItems.Where(predicate.Compile()).ToList();
        }

        public static List<TEntity> FilterDirectByEF<TEntity>(this ICollection<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration)
            where TEntity : class
        {
            var predicate = filter.CreateFilterPredicate(configuration) ?? (x => true);
            return FilterByEF(testItems, predicate);
        }

        public static List<TEntity> FilterNetCloneByEF<TEntity>(this ICollection<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration)
            where TEntity : class
        {
            var predicate = filter.Clone().CreateFilterPredicate(configuration) ?? (x => true);
            return FilterByEF(testItems, predicate);
        }

        public static List<TEntity> FilterNewtonCloneByEF<TEntity>(this ICollection<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration)
            where TEntity : class
        {
            var predicate = filter.NewtonsoftClone().CreateFilterPredicate(configuration) ?? (x => true);
            return FilterByEF(testItems, predicate);
        }

        private static EntityFilter<TEntity> NewtonsoftClone<TEntity>(this EntityFilter<TEntity> filter)
        {
            var serializerSettings = new JsonSerializerSettings { Converters = JsonConverterExtensions.FilterExpressionsNewtonsoftConverters };
            var json = JsonConvert.SerializeObject(filter, serializerSettings);
            return JsonConvert.DeserializeObject<EntityFilter<TEntity>>(json, serializerSettings);
        }

        private static List<TEntity> FilterByEF<TEntity>(this ICollection<TEntity> testItems, Expression<System.Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            using var dbContext = new TestDbContext<TEntity>();
            dbContext.Set<TEntity>().AddRange(testItems);
            dbContext.SaveChanges();

            return dbContext.Set<TEntity>().Where(predicate).ToList();
        }
    }
}
