using FS.SortQueryableCreator.Enums;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace FS.SortQueryableCreator.Tests.Models;

[ExcludeFromCodeCoverage]
public class SortTestcase<TModel> : SortTestcase
{
    public required Expression<Func<TModel, object?>> PropertySelector { get; set; }

    public required Func<IQueryable<TModel>, IOrderedQueryable<TModel>> ExpectedSortFunc { get; set; }

    public required SortDirection SortDirection { get; set; }

    public static SortTestcase<TModel> Create(string? syntax = null, Expression<Func<TModel, object?>>? propertySelector = null, SortDirection? sortDirection = SortDirection.Ascending, Func<IQueryable<TModel>, IOrderedQueryable<TModel>>? expectedSortFunc = null)
        => new()
        {
            ExpectedSortFunc = expectedSortFunc ?? (query => query.Order()),
            Syntax = syntax ?? default!,
            PropertySelector = propertySelector ?? (model => model),
            SortDirection = sortDirection ?? SortDirection.Ascending,
        };
}

[ExcludeFromCodeCoverage]
public abstract class SortTestcase
{
    public required string Syntax { get; set; }
}
