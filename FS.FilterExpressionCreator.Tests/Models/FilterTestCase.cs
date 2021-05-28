using FS.FilterExpressionCreator.Enums;
using FS.FilterExpressionCreator.Models;
using System;

namespace FS.FilterExpressionCreator.Tests.Models
{
    public class FilterTestCase<TFilterValue, TModelValue> : FilterTestCase
    {
        public FilterOperator FilterOperator { get; set; }
        public TFilterValue[] FilterValues { get; set; }
        public string FilterSyntax { get; set; }
        public Exception ExpectedException { get; set; }
        public Func<TModelValue, bool> ExpectedTestItemsExpression { get; set; }
        public FilterConfiguration FilterConfiguration { get; set; }

        private FilterTestCase(int id, FilterConfiguration filterConfiguration)
            : base(id)
        {
            FilterConfiguration = filterConfiguration;
        }

        public static FilterTestCase<TFilterValue, TModelValue> Create(int id, FilterOperator filterOperator, TFilterValue[] values, Func<TModelValue, bool> expectedTestItemsExpression, FilterConfiguration filterConfiguration = null)
            => new(id, filterConfiguration)
            {
                FilterOperator = filterOperator,
                FilterValues = values,
                ExpectedTestItemsExpression = expectedTestItemsExpression
            };

        public static FilterTestCase<TFilterValue, TModelValue> Create(int id, FilterOperator filterOperator, TFilterValue[] values, Exception expectedException = null, FilterConfiguration filterConfiguration = null)
            => new(id, filterConfiguration)
            {
                FilterOperator = filterOperator,
                FilterValues = values,
                ExpectedException = expectedException
            };

        public static FilterTestCase<TFilterValue, TModelValue> Create(int id, string filterSyntax, Func<TModelValue, bool> expectedTestItemsExpression, FilterConfiguration filterConfiguration = null)
            => new(id, filterConfiguration)
            {
                FilterSyntax = filterSyntax,
                ExpectedTestItemsExpression = expectedTestItemsExpression
            };

        public static FilterTestCase<TFilterValue, TModelValue> Create(int id, string filterSyntax, Exception expectedException = null, FilterConfiguration filterConfiguration = null)
            => new(id, filterConfiguration)
            {
                FilterSyntax = filterSyntax,
                ExpectedException = expectedException
            };
    }

    public class FilterTestCase
    {
        public int Id { get; set; }

        protected FilterTestCase(int id)
            => Id = id;

        public static FilterTestCase<TFilterValue, TModelValue> Create<TFilterValue, TModelValue>(int id, FilterOperator filterOperator, TFilterValue[] values, Func<TModelValue, bool> expectedTestItemsExpression, FilterConfiguration filterConfiguration = null)
            => FilterTestCase<TFilterValue, TModelValue>.Create(id, filterOperator, values, expectedTestItemsExpression, filterConfiguration);

        public static FilterTestCase<TFilterAndModelValue, TFilterAndModelValue> Create<TFilterAndModelValue>(int id, FilterOperator filterOperator, TFilterAndModelValue[] values, Exception expectedException = null, FilterConfiguration filterConfiguration = null)
            => FilterTestCase<TFilterAndModelValue, TFilterAndModelValue>.Create(id, filterOperator, values, expectedException, filterConfiguration);

        public static FilterTestCase<TFilterAndModelValue, TFilterAndModelValue> Create<TFilterAndModelValue>(int id, string filterSyntax, Func<TFilterAndModelValue, bool> expectedTestItemsExpression, FilterConfiguration filterConfiguration = null)
            => FilterTestCase<TFilterAndModelValue, TFilterAndModelValue>.Create(id, filterSyntax, expectedTestItemsExpression, filterConfiguration);

        public static FilterTestCase<TFilterAndModelValue, TFilterAndModelValue> Create<TFilterAndModelValue>(int id, string filterSyntax, Exception expectedException = null, FilterConfiguration filterConfiguration = null)
            => FilterTestCase<TFilterAndModelValue, TFilterAndModelValue>.Create(id, filterSyntax, expectedException, filterConfiguration);
    }
}
