using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Models
{
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
    [ExcludeFromCodeCoverage]
    public class TestModelNested
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ParentId { get; set; }

        public required string Value { get; set; }
    }
}
