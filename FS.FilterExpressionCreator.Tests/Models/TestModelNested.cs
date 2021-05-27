using System;

namespace FS.FilterExpressionCreator.Tests.Models
{
    public class TestModelNested
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ParentId { get; set; }

        public string Value { get; set; }
    }
}
