using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Models
{
    [ExcludeFromCodeCoverage]
    public class TestModelNested
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ParentId { get; set; }

        public string Value { get; set; }
    }
}
