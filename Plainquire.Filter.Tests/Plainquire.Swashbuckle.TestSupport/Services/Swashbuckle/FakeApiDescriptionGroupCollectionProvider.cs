// ReSharper disable All
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Collections.Generic;
using System.Linq;

namespace Swashbuckle.AspNetCore.SwaggerGen.Test
{
    public class FakeApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
    {
        private readonly IEnumerable<ApiDescription> _apiDescriptions;

        public FakeApiDescriptionGroupCollectionProvider(IEnumerable<ApiDescription> apiDescriptions)
        {
            _apiDescriptions = apiDescriptions;
        }

        public ApiDescriptionGroupCollection ApiDescriptionGroups
        {
            get
            {
                var apiDescriptionGroups = _apiDescriptions
                    .GroupBy(item => item.GroupName)
                    .Select(grouping => new ApiDescriptionGroup(grouping.Key, grouping.ToList()))
                    .ToList();

                return new ApiDescriptionGroupCollection(apiDescriptionGroups, 1);
            }
        }
    }
}