namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_HoofdactiviteitenLijst;

using AssociationRegistry.Test.Framework.Helpers;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_List_Of_Hoofdactiviteiten_With_Duplicates
{
    [Fact]
    public void Then_It_Throws_A_DuplicateHoofdactiviteitException()
    {
        var fixture = new Fixture();

        var hoofdactiviteiten = HoofdactiviteitVerenigingsloket.All()
                                                               .OrderBy(_ => fixture.Create<int>())
                                                               .Take(1)
                                                               .Repeat(2)
                                                               .ToArray();

        var ctor = () => HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteiten);

        ctor.Should().Throw<HoofdactiviteitIsDuplicaat>();
    }
}
