using FluentAssertions;
using FS.FilterExpressionCreator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FS.FilterExpressionCreator.Tests.Tests
{
    [TestClass, ExcludeFromCodeCoverage]
    public class SectionTests
    {
        [TestMethod]
        public void WhenSectionIsCreated_ResultMatchExpected()
        {
            var section1 = new Section<int>(1, 2);
            section1.Start.Should().Be(1);
            section1.End.Should().Be(2);

            var section2 = Section.Create(1, 2);
            section2.Start.Should().Be(1);
            section2.End.Should().Be(2);
        }

        [TestMethod]
        public void WhenSectionToStringIsCalled_ResultMatchExpected()
        {
            var section1 = new Section<int>(1, 2);
            section1.ToString().Should().Be("1_2");

            var section2 = Section.Create<NonConvertible>(default, default);
            section2.ToString().Should().Be(string.Empty);

            var section3 = Section.Create(new DateTime(2020, 1, 1), new DateTime(2020, 1, 1));
            section3.ToString().Should().Be("2020-01-01T00:00:00.0000000_2020-01-01T00:00:00.0000000");

            var section4 = Section.Create(new NonConvertible(), default);
            section4.ToString().Should().Be("NonConvertible");

            var section5 = Section.Create(default, new NonConvertible());
            section5.ToString().Should().Be("NonConvertible");

            var section6 = Section.Create(new NonConvertible(), new NonConvertible());
            section6.ToString().Should().Be("NonConvertible_NonConvertible");
        }

        [TestMethod]
        public void WhenSectionsAreCompared_ResultMatchExpected()
        {
            var sectionSmall1 = new Section<int>(2, 3);
            var sectionSmall2 = new Section<int>(2, 3);
            var sectionBig = new Section<int>(1, 4);

            (sectionSmall1 > sectionBig).Should().BeFalse();
            (sectionSmall1 >= sectionBig).Should().BeFalse();
            (sectionSmall1 < sectionBig).Should().BeTrue();
            (sectionSmall1 <= sectionBig).Should().BeTrue();

            (sectionSmall1 > sectionSmall2).Should().BeFalse();
            (sectionSmall1 >= sectionSmall2).Should().BeTrue();
            (sectionSmall1 < sectionSmall2).Should().BeFalse();
            (sectionSmall1 <= sectionSmall2).Should().BeTrue();
        }

        [TestMethod]
        public void WhenSectionIsConverted_ResultMatchExpected()
        {
            IConvertible sectionMin = new Section<double>(0, 0);
            IConvertible sectionAvg = new Section<double>(50, 75);
            IConvertible sectionMax = new Section<double>(double.MinValue, double.MaxValue);

            Action toBoolean = () => _ = sectionMax.ToBoolean(null);
            toBoolean.Should().Throw<InvalidCastException>().WithMessage("Invalid cast from Section to Boolean");

            Action toChar = () => _ = sectionMax.ToChar(null);
            toChar.Should().Throw<InvalidCastException>().WithMessage("Invalid cast from Section to Char");

            Action toDateTime = () => _ = sectionMax.ToDateTime(null);
            toDateTime.Should().Throw<InvalidCastException>().WithMessage("Invalid cast from Section to DateTime");

            sectionMin.ToString(null).Should().Be("0_0");
            sectionAvg.ToString(null).Should().Be("50_75");
            sectionMax.ToString(null).Should().Be("-1,7976931348623157E+308_1,7976931348623157E+308");

            sectionMin.ToByte(null).Should().Be(0);
            sectionAvg.ToByte(null).Should().Be(25);
            Action toByte = () => _ = sectionMax.ToByte(null);
            toByte.Should().Throw<OverflowException>().WithMessage("Section was too large for a Byte");

            sectionMin.ToDecimal(null).Should().Be(0);
            sectionAvg.ToDecimal(null).Should().Be(25);
            Action toDecimal = () => _ = sectionMax.ToDecimal(null);
            toDecimal.Should().Throw<OverflowException>().WithMessage("Section was too large for a Decimal");

            sectionMin.ToDouble(null).Should().Be(0);
            sectionAvg.ToDouble(null).Should().Be(25);
            sectionMax.ToDouble(null).Should().Be(double.PositiveInfinity);

            sectionMin.ToInt16(null).Should().Be(0);
            sectionAvg.ToInt16(null).Should().Be(25);
            Action toInt16 = () => _ = sectionMax.ToInt16(null);
            toInt16.Should().Throw<OverflowException>().WithMessage("Section was too large for a Int16");

            sectionMin.ToInt32(null).Should().Be(0);
            sectionAvg.ToInt32(null).Should().Be(25);
            Action toInt32 = () => _ = sectionMax.ToInt32(null);
            toInt32.Should().Throw<OverflowException>().WithMessage("Section was too large for a Int32");

            sectionMin.ToInt64(null).Should().Be(0);
            sectionAvg.ToInt64(null).Should().Be(25);
            Action toInt64 = () => _ = sectionMax.ToInt64(null);
            toInt64.Should().Throw<OverflowException>().WithMessage("Section was too large for a Int64");

            sectionMin.ToSByte(null).Should().Be(0);
            sectionAvg.ToSByte(null).Should().Be(25);
            Action toSByte = () => _ = sectionMax.ToSByte(null);
            toSByte.Should().Throw<OverflowException>().WithMessage("Section was too large for a SByte");

            sectionMin.ToSingle(null).Should().Be(0);
            sectionAvg.ToSingle(null).Should().Be(25);
            Action toSingle = () => _ = sectionMax.ToSingle(null);
            toSingle.Should().Throw<OverflowException>().WithMessage("Section was too large for a Single");

            sectionMin.ToUInt16(null).Should().Be(0);
            sectionAvg.ToUInt16(null).Should().Be(25);
            Action toUInt16 = () => _ = sectionMax.ToUInt16(null);
            toUInt16.Should().Throw<OverflowException>().WithMessage("Section was too large for a UInt16");

            sectionMin.ToUInt32(null).Should().Be(0);
            sectionAvg.ToUInt32(null).Should().Be(25);
            Action toUInt32 = () => _ = sectionMax.ToUInt32(null);
            toUInt32.Should().Throw<OverflowException>().WithMessage("Section was too large for a UInt32");

            sectionMin.ToUInt64(null).Should().Be(0);
            sectionAvg.ToUInt64(null).Should().Be(25);
            Action toUInt64 = () => _ = sectionMax.ToUInt64(null);
            toUInt64.Should().Throw<OverflowException>().WithMessage("Section was too large for a UInt64");

            sectionMin.ToType(typeof(ulong), null).Should().Be(0);
            sectionAvg.ToType(typeof(ulong), null).Should().Be(25);
            Action toType = () => _ = sectionMax.ToType(typeof(ulong), null);
            toType.Should().Throw<OverflowException>().WithMessage("Section was too large for a UInt64");
        }

        [TestMethod]
        public void SectionDistance_ShouldMatchExpected()
        {
            var intSection = new Section<int>(1, 4);
            intSection.Distance.Should().Be(3);

            var dateTimeSection = new Section<DateTime>(new DateTime(2020, 1, 1), new DateTime(2020, 1, 2));
            dateTimeSection.Distance.Should().Be(TimeSpan.FromDays(1).Ticks);

            var dateTimeOffsetSection = new Section<DateTimeOffset>(new DateTime(2020, 1, 1), new DateTime(2020, 1, 2));
            dateTimeOffsetSection.Distance.Should().Be(TimeSpan.FromDays(1).Ticks);
        }

        [TestMethod]
        public void WhenStartAndEndOfSectionAreNull_DistanceIsZero()
        {
            var section = new Section<NonConvertible>(null, null);
            section.Distance.Should().Be(0);
        }

        [TestMethod]
        public void SectionWithNonConvertibleType_CouldNotCalculateDistance()
        {
            var section = new Section<NonConvertible>(new NonConvertible(), new NonConvertible());
            Action toDouble = () => _ = section.Distance;
            toDouble.Should().Throw<InvalidOperationException>().WithMessage("The type NonConvertible is not convertible to Double");
        }

        [TestMethod]
        public void SectionWithNonConvertibleType_CouldNotConvertedViaIConvertible()
        {
            IConvertible section = new Section<NonConvertible>(new NonConvertible(), new NonConvertible());
            Action toDouble = () => _ = section.ToDouble(null);
            toDouble.Should().Throw<InvalidOperationException>().WithMessage("The type NonConvertible is not convertible to Double");
        }

        [TestMethod]
        public void Section_HasTypeCodeObject()
        {
            IConvertible section = new Section<int>(2, 3);
            section.GetTypeCode().Should().Be(TypeCode.Object);
        }

        [TestMethod]
        public void WhenSectionHasCodeIsCalculated_ResultMatchExpected()
        {
            IConvertible section = new Section<int>(2, 3);
            section.GetHashCode().Should().Be(HashCode.Combine(2, 3));
        }

        [TestMethod]
        public void WhenDateTimeSpansComparedForEquality_ResultMatchExpected()
        {
            var spanA1 = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA2 = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB = new Section<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));

            spanA1.Should().Be(spanA2);
            spanA2.Should().NotBe(spanB);
        }

        [TestMethod]
        public void WhenDateTimeSpansIntersectIsCalculated_ResultMatchExpected()
        {
            // ReSharper disable InconsistentNaming
            var spanA_A = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_B = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_C = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_B = new Section<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_C = new Section<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_D = new Section<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanD_D = new Section<DateTimeOffset>(new DateTimeOffset(2030, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
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
            var spanA_A = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_B = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_C = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_B = new Section<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_C = new Section<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_D = new Section<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanD_D = new Section<DateTimeOffset>(new DateTimeOffset(2030, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
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
            var spanA_A = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2000, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_B = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanA_C = new Section<DateTimeOffset>(new DateTimeOffset(2000, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_B = new Section<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2010, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_C = new Section<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2020, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanB_D = new Section<DateTimeOffset>(new DateTimeOffset(2010, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
            var spanD_D = new Section<DateTimeOffset>(new DateTimeOffset(2030, 01, 01, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2030, 12, 31, 0, 0, 0, TimeSpan.Zero));
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

        private class NonConvertible : IComparable<NonConvertible>
        {
            public int CompareTo(NonConvertible other)
                => throw new NotImplementedException();

            public override string ToString()
                => "NonConvertible";
        }
    }
}
