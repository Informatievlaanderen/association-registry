namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_HoofdactiviteitenLijst;

using AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_A_List_Of_Hoofdactiviteiten
{
    [Fact]
    public void Then_It_Returns_A_Filled_HoofdactiviteitenLijst()
    {
        var fixture = new Fixture();

        var hoofdactiviteiten = HoofdactiviteitVerenigingsloket.All()
                                                               .OrderBy(_ => fixture.Create<int>())
                                                               .Take(2)
                                                               .ToArray();

        var hoofdactiviteitenLijst = HoofdactiviteitenVerenigingsloket.FromArray(hoofdactiviteiten);

        hoofdactiviteitenLijst.Should().BeEquivalentTo(hoofdactiviteiten);
    }
}
