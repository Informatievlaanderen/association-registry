﻿namespace AssociationRegistry.Test.When_Creating_A_WerkingsgebiedenLijst;

using AutoFixture;
using FluentAssertions;
using Framework.Helpers;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Werkingsgebieden_With_Duplicates
{
    [Fact]
    public void Then_It_Throws_A_DuplicateWerkingsgebiedException()
    {
        var fixture = new Fixture();

        var werkingsgebieden = Werkingsgebied.All
                                                               .OrderBy(_ => fixture.Create<int>())
                                                               .Take(1)
                                                               .Repeat(2)
                                                               .ToArray();

        var ctor = () => Werkingsgebieden.FromArray(werkingsgebieden);

        ctor.Should().Throw<WerkingsgebiedIsDuplicaat>();
    }
}