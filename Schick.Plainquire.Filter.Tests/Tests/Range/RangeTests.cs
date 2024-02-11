using FluentAssertions;
using FluentAssertions.Execution;
using Schick.Plainquire.Filter.Abstractions.Extensions;
using Schick.Plainquire.Filter.Abstractions.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Schick.Plainquire.Filter.Tests.Tests.Range;

[TestClass, ExcludeFromCodeCoverage]
public class RangeTests
{
    [TestMethod]
    public void WhenRangeIsCreated_ResultMatchExpected()
    {
        var range1 = new Range<int>(1, 2);
        range1.Start.Should().Be(1);
        range1.End.Should().Be(2);

        var range2 = Abstractions.Models.Range.Create(1, 2);
        range2.Start.Should().Be(1);
        range2.End.Should().Be(2);
    }

    [TestMethod]
    public void WhenRangeToStringIsCalled_ResultMatchExpected()
    {
        using var _ = new AssertionScope();

        var range1 = new Range<int>(1, 2);
        range1.ToString().Should().Be("1_2");

        var range2 = Abstractions.Models.Range.Create<NonConvertible>(default, default);
        range2.ToString().Should().Be("_");

        var range3 = Abstractions.Models.Range.Create(new DateTime(2020, 1, 1), new DateTime(2020, 1, 1));
        range3.ToString().Should().Be("2020-01-01T00:00:00.0000000_2020-01-01T00:00:00.0000000");

        var range4 = Abstractions.Models.Range.Create(new NonConvertible(), default);
        range4.ToString().Should().Be("NonConvertible_");

        var range5 = Abstractions.Models.Range.Create(default, new NonConvertible());
        range5.ToString().Should().Be("_NonConvertible");

        var range6 = Abstractions.Models.Range.Create(new NonConvertible(), new NonConvertible());
        range6.ToString().Should().Be("NonConvertible_NonConvertible");
    }

    [TestMethod]
    public void WhenRangesAreCompared_ResultMatchExpected()
    {
        var rangeSmall1 = new Range<int>(2, 3);
        var rangeSmall2 = new Range<int>(2, 3);
        var rangeBig = new Range<int>(1, 4);

        (rangeSmall1 > rangeBig).Should().BeFalse();
        (rangeSmall1 >= rangeBig).Should().BeFalse();
        (rangeSmall1 < rangeBig).Should().BeTrue();
        (rangeSmall1 <= rangeBig).Should().BeTrue();

        (rangeSmall1 > rangeSmall2).Should().BeFalse();
        (rangeSmall1 >= rangeSmall2).Should().BeTrue();
        (rangeSmall1 < rangeSmall2).Should().BeFalse();
        (rangeSmall1 <= rangeSmall2).Should().BeTrue();
    }

    [TestMethod]
    public void WhenRangeIsConverted_ResultMatchExpected()
    {
        IConvertible rangeMin = new Range<double>(0, 0);
        IConvertible rangeAvg = new Range<double>(50, 75);
        IConvertible rangeMax = new Range<double>(double.MinValue, double.MaxValue);

        using var __ = new AssertionScope();

        Action toBoolean = () => _ = rangeMax.ToBoolean(null);
        toBoolean.Should().Throw<InvalidCastException>().WithMessage("Invalid cast from Range to Boolean");

        Action toChar = () => _ = rangeMax.ToChar(null);
        toChar.Should().Throw<InvalidCastException>().WithMessage("Invalid cast from Range to Char");

        Action toDateTime = () => _ = rangeMax.ToDateTime(null);
        toDateTime.Should().Throw<InvalidCastException>().WithMessage("Invalid cast from Range to DateTime");

        rangeMin.ToString(null).Should().Be("0_0");
        rangeAvg.ToString(null).Should().Be("50_75");
        // Flaky
        //rangeMax.ToString(new CultureInfo("en-US")).Should().Be("-1,7976931348623157E+308_1,7976931348623157E+308");

        rangeMin.ToByte(null).Should().Be(0);
        rangeAvg.ToByte(null).Should().Be(25);
        Action toByte = () => _ = rangeMax.ToByte(null);
        toByte.Should().Throw<OverflowException>().WithMessage("Range was too large for a Byte");

        rangeMin.ToDecimal(null).Should().Be(0);
        rangeAvg.ToDecimal(null).Should().Be(25);
        Action toDecimal = () => _ = rangeMax.ToDecimal(null);
        toDecimal.Should().Throw<OverflowException>().WithMessage("Range was too large for a Decimal");

        rangeMin.ToDouble(null).Should().Be(0);
        rangeAvg.ToDouble(null).Should().Be(25);
        rangeMax.ToDouble(null).Should().Be(double.PositiveInfinity);

        rangeMin.ToInt16(null).Should().Be(0);
        rangeAvg.ToInt16(null).Should().Be(25);
        Action toInt16 = () => _ = rangeMax.ToInt16(null);
        toInt16.Should().Throw<OverflowException>().WithMessage("Range was too large for a Int16");

        rangeMin.ToInt32(null).Should().Be(0);
        rangeAvg.ToInt32(null).Should().Be(25);
        Action toInt32 = () => _ = rangeMax.ToInt32(null);
        toInt32.Should().Throw<OverflowException>().WithMessage("Range was too large for a Int32");

        rangeMin.ToInt64(null).Should().Be(0);
        rangeAvg.ToInt64(null).Should().Be(25);
        Action toInt64 = () => _ = rangeMax.ToInt64(null);
        toInt64.Should().Throw<OverflowException>().WithMessage("Range was too large for a Int64");

        rangeMin.ToSByte(null).Should().Be(0);
        rangeAvg.ToSByte(null).Should().Be(25);
        Action toSByte = () => _ = rangeMax.ToSByte(null);
        toSByte.Should().Throw<OverflowException>().WithMessage("Range was too large for a SByte");

        rangeMin.ToSingle(null).Should().Be(0);
        rangeAvg.ToSingle(null).Should().Be(25);
        Action toSingle = () => _ = rangeMax.ToSingle(null);
        toSingle.Should().Throw<OverflowException>().WithMessage("Range was too large for a Single");

        rangeMin.ToUInt16(null).Should().Be(0);
        rangeAvg.ToUInt16(null).Should().Be(25);
        Action toUInt16 = () => _ = rangeMax.ToUInt16(null);
        toUInt16.Should().Throw<OverflowException>().WithMessage("Range was too large for a UInt16");

        rangeMin.ToUInt32(null).Should().Be(0);
        rangeAvg.ToUInt32(null).Should().Be(25);
        Action toUInt32 = () => _ = rangeMax.ToUInt32(null);
        toUInt32.Should().Throw<OverflowException>().WithMessage("Range was too large for a UInt32");

        rangeMin.ToUInt64(null).Should().Be(0);
        rangeAvg.ToUInt64(null).Should().Be(25);
        Action toUInt64 = () => _ = rangeMax.ToUInt64(null);
        toUInt64.Should().Throw<OverflowException>().WithMessage("Range was too large for a UInt64");

        rangeMin.ToType(typeof(ulong), null).Should().Be(0);
        rangeAvg.ToType(typeof(ulong), null).Should().Be(25);
        Action toType = () => _ = rangeMax.ToType(typeof(ulong), null);
        toType.Should().Throw<OverflowException>().WithMessage("Range was too large for a UInt64");
    }

    [TestMethod]
    public void RangeDistance_ShouldMatchExpected()
    {
        var intRange = new Range<int>(1, 4);
        intRange.Distance.Should().Be(3);

        var dateTimeRange = new Range<DateTime>(new DateTime(2020, 1, 1), new DateTime(2020, 1, 2));
        dateTimeRange.Distance.Should().Be(TimeSpan.FromDays(1).Ticks);

        var dateTimeOffsetRange = new Range<DateTimeOffset>(new DateTime(2020, 1, 1), new DateTime(2020, 1, 2));
        dateTimeOffsetRange.Distance.Should().Be(TimeSpan.FromDays(1).Ticks);
    }

    [TestMethod]
    public void WhenStartAndEndOfRangeAreNull_DistanceIsZero()
    {
        var range = new Range<NonConvertible>(null, null);
        range.Distance.Should().Be(0);
    }

    [TestMethod]
    public void RangeWithNonConvertibleType_CouldNotCalculateDistance()
    {
        var range = new Range<NonConvertible>(new NonConvertible(), new NonConvertible());
        Action toDouble = () => _ = range.Distance;
        toDouble.Should().Throw<InvalidOperationException>().WithMessage("The type NonConvertible is not convertible to Double");
    }

    [TestMethod]
    public void RangeWithNonConvertibleType_CouldNotConvertedViaIConvertible()
    {
        IConvertible range = new Range<NonConvertible>(new NonConvertible(), new NonConvertible());
        Action toDouble = () => _ = range.ToDouble(null);
        toDouble.Should().Throw<InvalidOperationException>().WithMessage("The type NonConvertible is not convertible to Double");
    }

    [TestMethod]
    public void Range_HasTypeCodeObject()
    {
        IConvertible range = new Range<int>(2, 3);
        range.GetTypeCode().Should().Be(TypeCode.Object);
    }

    [TestMethod]
    public void WhenRangeHasCodeIsCalculated_ResultMatchExpected()
    {
        IConvertible range = new Range<int>(2, 3);
        range.GetHashCode().Should().Be(HashCode.Combine(2, 3));
    }

    [TestMethod]
    public void WhenDateTimeSpansComparedForEquality_ResultMatchExpected()
    {
        var spanA1 = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanA2 = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));

        spanA1.Should().Be(spanA2);
        spanA2.Should().NotBe(spanB);
    }

    [TestMethod]
    public void WhenDateTimeSpansIntersectIsCalculated_ResultMatchExpected()
    {
        // ReSharper disable InconsistentNaming
        var spanA_A = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanA_B = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanA_C = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_B = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_C = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_D = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanD_D = new Range<DateTimeOffset>(new DateTimeOffset(2030, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
        // ReSharper restore InconsistentNaming

        spanA_B.Intersect(spanA_A).Should().BeTrue();
        spanA_A.Intersect(spanA_B).Should().BeTrue();

        spanA_C.Intersect(spanB_B).Should().BeTrue();
        spanB_B.Intersect(spanA_C).Should().BeTrue();

        spanA_C.Intersect(spanB_D).Should().BeTrue();
        spanB_D.Intersect(spanA_C).Should().BeTrue();

        spanB_C.Intersect(spanD_D).Should().BeFalse();
        spanD_D.Intersect(spanB_C).Should().BeFalse();

        spanA_A.Intersect(null).Should().BeFalse();
        ((Range<DateTimeOffset>?)null).Intersect(spanA_A).Should().BeFalse();
        ((Range<DateTimeOffset>?)null).Intersect(null).Should().BeFalse();
    }

    [TestMethod]
    public void OverlappingRangesWithOpenSide_DoIntersect()
    {
        var overlappingRanges = new[] {
            new {
                Id = 1,
                RangeA =  new Range<string>(default, default),
                RangeB =  new Range<string>(default, default)
            },
            new {
                Id = 2,
                RangeA =  new Range<string>("10", default),
                RangeB =  new Range<string>("10", default)
            },
            new {
                Id = 3,
                RangeA =  new Range<string>(default, "10"),
                RangeB =  new Range<string>(default, "10")
            },

            new {
                Id = 4,
                RangeA =  new Range<string>(default, "10"),
                RangeB =  new Range<string>("10", default)
            },
            new {
                Id = 5,
                RangeA =  new Range<string>("10", "10"),
                RangeB =  new Range<string>("10", default)
            },
            new {
                Id = 6,
                RangeA =  new Range<string>("10", "10"),
                RangeB =  new Range<string>(default, "10")
            }
        };

        using var _ = new AssertionScope();
        foreach (var ranges in overlappingRanges)
        {
            ranges.RangeA.Intersect(ranges.RangeB).Should().BeTrue(because: $"ranges with Id {ranges.Id} overlaps");
            ranges.RangeB.Intersect(ranges.RangeA).Should().BeTrue(because: $"ranges with Id {ranges.Id} overlaps");
        }
    }

    [TestMethod]
    public void DistinctRangesWithOpenSide_DoNotIntersect()
    {
        var distinctRanges = new[] {
            new {
                Id = 1,
                RangeA =  new Range<string>(default, "10"),
                RangeB =  new Range<string>("11", default)
            },
            new {
                Id = 2,
                RangeA =  new Range<string>("10", "10"),
                RangeB =  new Range<string>("11", default)
            },
            new {
                Id = 3,
                RangeA =  new Range<string>("10", "10"),
                RangeB =  new Range<string>(default, "09")
            }
        };

        using var _ = new AssertionScope();
        foreach (var ranges in distinctRanges)
        {
            ranges.RangeA.Intersect(ranges.RangeB).Should().BeFalse(because: $"ranges with Id {ranges.Id} are distinct");
            ranges.RangeB.Intersect(ranges.RangeA).Should().BeFalse(because: $"ranges with Id {ranges.Id} are distinct");
        }
    }

    [TestMethod]
    public void WhenDateTimeSpansIntersectionIsCalculated_ResultMatchExpected()
    {
        // ReSharper disable InconsistentNaming
        var spanA_A = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanA_B = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanA_C = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_B = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_C = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_D = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanD_D = new Range<DateTimeOffset>(new DateTimeOffset(2030, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
        // ReSharper restore InconsistentNaming

        spanA_B.Intersection(spanA_A).Should().Be(spanA_A);
        spanA_A.Intersection(spanA_B).Should().Be(spanA_A);

        spanA_C.Intersection(spanB_B).Should().Be(spanB_B);
        spanB_B.Intersection(spanA_C).Should().Be(spanB_B);

        spanA_C.Intersection(spanB_D).Should().Be(spanB_C);
        spanB_D.Intersection(spanA_C).Should().Be(spanB_C);

        spanB_C.Intersection(spanD_D).Should().Be(default);
        spanD_D.Intersection(spanB_C).Should().Be(default);

        spanA_A.Intersection(null).Should().Be(spanA_A);
        ((Range<DateTimeOffset>?)null).Intersection(spanA_A).Should().Be(spanA_A);
        ((Range<DateTimeOffset>?)null).Intersection(null).Should().Be(default);
    }

    [TestMethod]
    public void WhenDateTimeSpansUnionIsCalculated_ResultMatchExpected()
    {
        // ReSharper disable InconsistentNaming
        var spanA_A = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanA_B = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanA_C = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanA_D = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_B = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_C = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_D = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanD_D = new Range<DateTimeOffset>(new DateTimeOffset(2030, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
        // ReSharper restore InconsistentNaming

        spanA_B.Union(spanA_A).Should().Be(spanA_B);
        spanA_A.Union(spanA_B).Should().Be(spanA_B);

        spanA_C.Union(spanB_B).Should().Be(spanA_C);
        spanB_B.Union(spanA_C).Should().Be(spanA_C);

        spanA_C.Union(spanB_D).Should().Be(spanA_D);
        spanB_D.Union(spanA_C).Should().Be(spanA_D);

        spanB_C.Union(spanD_D).Should().Be(spanB_D);
        spanD_D.Union(spanB_C).Should().Be(spanB_D);

        spanA_A.Union(null).Should().Be(spanA_A);
        ((Range<DateTimeOffset>?)null).Union(spanA_A).Should().Be(spanA_A);
        ((Range<DateTimeOffset>?)null).Union(null).Should().Be(default);
    }

    [TestMethod]
    public void WhenDateTimeSpansContainsIsCalculated_ResultMatchExpected()
    {
        // ReSharper disable InconsistentNaming
        var spanA_A = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanA_B = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanA_C = new Range<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_B = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_C = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanB_D = new Range<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
        var spanD_D = new Range<DateTimeOffset>(new DateTimeOffset(2030, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
        // ReSharper restore InconsistentNaming

        spanA_B.Contains(spanA_A).Should().BeTrue();
        spanA_A.Contains(spanA_B).Should().BeFalse();

        spanA_C.Contains(spanB_B).Should().BeTrue();
        spanB_B.Contains(spanA_C).Should().BeFalse();

        spanA_C.Contains(spanB_D).Should().BeFalse();
        spanB_D.Contains(spanA_C).Should().BeFalse();

        spanB_C.Contains(spanD_D).Should().BeFalse();
        spanD_D.Contains(spanB_C).Should().BeFalse();

        spanA_A.Contains(null).Should().BeFalse();
        ((Range<DateTimeOffset>?)null).Contains(spanA_A).Should().BeFalse();
        ((Range<DateTimeOffset>?)null).Contains(null).Should().BeFalse();
    }

    [TestMethod]
    public void EqualRangesWithOpenSide_ContainsItselfOther()
    {
        var containingRanges = new[] {
            new {
                Id = 1,
                Range =  new Range<string>(default, default)
            },
            new {
                Id = 2,
                Range =  new Range<string>("10", default)
            },
            new {
                Id = 3,
                Range =  new Range<string>(default, "10")
            }
        };

        using var _ = new AssertionScope();
        foreach (var ranges in containingRanges)
            ranges.Range.Contains(ranges.Range).Should().BeTrue(because: $"range with Id {ranges.Id} should contain itself");
    }

    [TestMethod]
    public void ContainerRangeWithOpenSide_ContainsInner()
    {
        var containingRanges = new[] {
            new {
                Id = 1,
                Outer =  new Range<string>(default, default),
                Inner =  new Range<string>("10", default)
            },
            new {
                Id = 2,
                Outer =  new Range<string>(default, default),
                Inner =  new Range<string>(default, "10")
            },
            new {
                Id = 3,
                Outer =  new Range<string>(default, default),
                Inner =  new Range<string>("10", "10")
            },
            new {
                Id = 4,
                Outer =  new Range<string>("10", default),
                Inner =  new Range<string>("11", default)
            },
            new {
                Id = 5,
                Outer =  new Range<string>(default, "11"),
                Inner =  new Range<string>(default, "10")
            }
        };

        using var _ = new AssertionScope();
        foreach (var ranges in containingRanges)
        {
            ranges.Outer.Contains(ranges.Inner).Should().BeTrue(because: $"container range with Id {ranges.Id} should contain inner range");
            ranges.Inner.Contains(ranges.Outer).Should().BeFalse(because: $"inner range with Id {ranges.Id} should not contain container range");
        }
    }

    private class NonConvertible : IComparable<NonConvertible>
    {
        public int CompareTo(NonConvertible? other)
            => throw new NotImplementedException();

        public override string ToString()
            => "NonConvertible";
    }
}