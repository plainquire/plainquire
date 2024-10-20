﻿using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Plainquire.Page.Tests.Tests.EntityPage;

[TestFixture]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Created by reflection")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Local", Justification = "Accessed by reflection")]
public class EntityPageCastTests
{
    [Test]
    public void WhenTypedEntityPageIsCast_PageParametersAreKept()
    {
        var dtoPage = new EntityPage<PersonDto>(2, 3);
        var modelPage = dtoPage.Cast<PersonModel>();

        using var _ = new AssertionScope();
        modelPage.PageNumberValue.Should().Be("2");
        modelPage.PageSizeValue.Should().Be("3");
    }

    [Test]
    public void WhenEntityPageIsCloned_PageParametersAreKept()
    {
        var page = new Page.EntityPage(2, 3);
        var clone = (Page.EntityPage)page.Clone();

        using var _ = new AssertionScope();
        clone.PageNumberValue.Should().Be("2");
        clone.PageSizeValue.Should().Be("3");
    }

    private record PersonModel(string FullName);

    private record PersonDto(string FullName, string FirstName, string LastName);
}