using FluentAssertions;
using FluentAssertions.Execution;
using FS.SortQueryableCreator.Sorts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace FS.SortQueryableCreator.Tests.Tests.EntitySort;

[TestClass, ExcludeFromCodeCoverage]
public class EntitySortCastTests
{
    [TestMethod]
    public void WhenEntitySortIsCast_PropertiesAreMappedByName()
    {
        var sort = new EntitySort<PersonDto>()
            .Add(x => x.Name)
            .Add(x => x.NameDtoOnly)
            .Add(x => x.Address)
            .Add(x => x.AddressDtoOnly)
            .Add(x => x.Address.Street)
            .Add(x => x.Address.StreetDtoOnly)
            .Add(x => x.Address.Country.Name)
            .Add(x => x.Address.Country.NameDtoOnly);

        var castSort = sort.Cast<PersonModel>();

        using var _ = new AssertionScope();
        castSort._propertySorts.Should().HaveCount(4);
        castSort._propertySorts.Should().Contain(x => x.PropertyPath == "Name");
        castSort._propertySorts.Should().Contain(x => x.PropertyPath == "Address");
        castSort._propertySorts.Should().Contain(x => x.PropertyPath == "Address.Street");
        castSort._propertySorts.Should().Contain(x => x.PropertyPath == "Address.Country.Name");
    }

    public record PersonModel(string Name, AddressModel Address);
    public record AddressModel(string Street, CountryModel Country);
    public record CountryModel(string Name);

    public record PersonDto(string Name, string NameDtoOnly, AddressDto Address, AddressDto AddressDtoOnly);
    public record AddressDto(string Street, string StreetDtoOnly, CountryDto Country);
    public record CountryDto(string Name, string NameDtoOnly);
}