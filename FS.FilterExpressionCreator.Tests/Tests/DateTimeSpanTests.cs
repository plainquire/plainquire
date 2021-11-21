using FluentAssertions;
using FS.FilterExpressionCreator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FS.FilterExpressionCreator.Tests.Tests
{
    [TestClass]
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
        public void WhenDateTimeSpansIntersects_IntersectReturnsTrue()
        {
            var earlySpan = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var laterSpan = new DateTimeSpan(new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));

            earlySpan.Should().Match<DateTimeSpan>(early => early.Intersect(laterSpan));
        }

        [TestMethod]
        public void WhenDateTimeSpansNotIntersects_IntersectReturnsFalse()
        {
            var earlySpan = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var laterSpan = new DateTimeSpan(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));

            earlySpan.Should().Match<DateTimeSpan>(early => !early.Intersect(laterSpan));
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
        public void WhenDateTimeSpanContainsOther_ContainsReturnsTrue()
        {
            var outer = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var inner = new DateTimeSpan(new DateTimeOffset(2000, 02, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 11, 30, 0, 0, 0, TimeSpan.Zero));

            outer.Should().Match<DateTimeSpan>(o => o.Contains(inner));
            inner.Should().Match<DateTimeSpan>(i => !i.Contains(outer));
        }

        [TestMethod]
        public void WhenDateTimeSpansNotIntersects_TheyDoesNotContainsEachOther()
        {
            var outer = new DateTimeSpan(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var inner = new DateTimeSpan(new DateTimeOffset(2000, 02, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));

            outer.Should().Match<DateTimeSpan>(o => !o.Contains(inner));
            inner.Should().Match<DateTimeSpan>(i => !i.Contains(outer));
        }
    }
}
