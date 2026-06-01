namespace AssociationRegistry.Test.Erkenningen.When_Creating_Te_Wijzigen_Erkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Primitives;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Geen_Te_Wijzigen_Waarden
{
    private readonly Fixture _fixture;

    public Given_Geen_Te_Wijzigen_Waarden()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Fact]
    public void Then_Throws_MinstensEenVeldMoetIngevuldZijn()
    {
        var redenVanWijziging = _fixture.Create<string>();
        var exception = Assert.Throws<MinstensEenVeldMoetIngevuldZijn>(() => TeWijzigenErkenning.Create(
                                                                           _fixture.Create<int>(),
                                                                           NullOrEmpty<DateOnly>.Null,
                                                                           NullOrEmpty<DateOnly>.Null,
                                                                           NullOrEmpty<DateOnly>.Null,
                                                                           null,
                                                                           redenVanWijziging));

        exception.Message.Should().Be(ExceptionMessages.MinstensEenVeldMoetIngevuldZijn);
    }
}
