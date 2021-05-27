using System;
using System.Collections.Generic;

namespace FS.FilterExpressionCreator.Tests.Models
{
    public class TestModel<TValue>
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public TValue ValueA { get; set; }

        public TValue ValueB { get; set; }

        public TestModelNested NestedObject { get; set; }

        public List<TestModelNested> NestedList { get; set; }
    }
}
