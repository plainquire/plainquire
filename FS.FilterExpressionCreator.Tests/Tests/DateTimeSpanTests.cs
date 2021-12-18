using FluentAssertions;
using FS.FilterExpressionCreator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class DateTimeSpanTests
    {
        [TestMethod]
        public void WhenDateTimeSpansComparedForEquality_ResultMatchExpected()
        {
            var spanA1 = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA2 = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanDefault = new DateTimeSpan(DateTimeOffset.MinValue, DateTimeOffset.MinValue);

            spanA1.Should().Be(spanA2);
            spanA2.Should().NotBe(spanB);
            spanDefault.Should().Be(default(DateTimeSpan));
        }

        [TestMethod]
        public void WhenDateTimeSpansIntersectIsCalculated_ResultMatchExpected()
        {
            // ReSharper disable InconsistentNaming
            var spanA_A = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_B = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_C = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_B = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_C = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_D = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanD_D = new DateTimeSpan(new DateTimeOffset(2030, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
            // ReSharper restore InconsistentNaming

            spanA_B.Intersect(spanA_A).Should().BeTrue();
            spanA_A.Intersect(spanA_B).Should().BeTrue();

            spanA_C.Intersect(spanB_B).Should().BeTrue();
            spanB_B.Intersect(spanA_C).Should().BeTrue();

            spanA_C.Intersect(spanB_D).Should().BeTrue();
            spanB_D.Intersect(spanA_C).Should().BeTrue();

            spanB_C.Intersect(spanD_D).Should().BeFalse();
            spanD_D.Intersect(spanB_C).Should().BeFalse();
        }

        [TestMethod]
        public void WhenDateTimeSpansIntersectionIsCalculated_ResultMatchExpected()
        {
            // ReSharper disable InconsistentNaming
            var spanA_A = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_B = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_C = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_B = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_C = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_D = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanD_D = new DateTimeSpan(new DateTimeOffset(2030, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
            // ReSharper restore InconsistentNaming

            spanA_B.Intersection(spanA_A).Should().Be(spanA_A);
            spanA_A.Intersection(spanA_B).Should().Be(spanA_A);

            spanA_C.Intersection(spanB_B).Should().Be(spanB_B);
            spanB_B.Intersection(spanA_C).Should().Be(spanB_B);

            spanA_C.Intersection(spanB_D).Should().Be(spanB_C);
            spanB_D.Intersection(spanA_C).Should().Be(spanB_C);

            spanB_C.Intersection(spanD_D).Should().BeNull();
            spanD_D.Intersection(spanB_C).Should().BeNull();
        }

        [TestMethod]
        public void WhenDateTimeSpansContainsIsCalculated_ResultMatchExpected()
        {
            // ReSharper disable InconsistentNaming
            var spanA_A = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_B = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_C = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_B = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_C = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_D = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanD_D = new DateTimeSpan(new DateTimeOffset(2030, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
            // ReSharper restore InconsistentNaming

            spanA_B.Contains(spanA_A).Should().BeTrue();
            spanA_A.Contains(spanA_B).Should().BeFalse();

            spanA_C.Contains(spanB_B).Should().BeTrue();
            spanB_B.Contains(spanA_C).Should().BeFalse();

            spanA_C.Contains(spanB_D).Should().BeFalse();
            spanB_D.Contains(spanA_C).Should().BeFalse();

            spanB_C.Contains(spanD_D).Should().BeFalse();
            spanD_D.Contains(spanB_C).Should().BeFalse();
        }
    }
}
