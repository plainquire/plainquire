using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Sort.Tests.Tests.EntitySort;

[TestClass]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
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
        castSort.PropertySorts.Should().HaveCount(4);
        castSort.PropertySorts.Should().Contain(x => x.PropertyPath == "Name");
        castSort.PropertySorts.Should().Contain(x => x.PropertyPath == "Address");
        castSort.PropertySorts.Should().Contain(x => x.PropertyPath == "Address.Street");
        castSort.PropertySorts.Should().Contain(x => x.PropertyPath == "Address.Country.Name");
    }

    private record PersonModel(string Name, AddressModel Address);
    private record AddressModel(string Street, CountryModel Country);
    private record CountryModel(string Name);

    private record PersonDto(string Name, string NameDtoOnly, AddressDto Address, AddressDto AddressDtoOnly);
    private record AddressDto(string Street, string StreetDtoOnly, CountryDto Country);
    private record CountryDto(string Name, string NameDtoOnly);
}