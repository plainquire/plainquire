using FS.FilterExpressionCreator.Models;
using System.Collections.Generic;

namespace FS.FilterExpressionCreator.Tests.Models
{
    public delegate List<TEntity> EntityFilterFunc<TEntity>(ICollection<TEntity> testItems, EntityFilter<TEntity> filter, FilterConfiguration configuration = null);

    public delegate List<TestModel<TModelValue>> TestModelFilterFunc<TModelValue>(ICollection<TestModel<TModelValue>> testItems, EntityFilter<TestModel<TModelValue>> filter, FilterConfiguration configuration = null);
}
