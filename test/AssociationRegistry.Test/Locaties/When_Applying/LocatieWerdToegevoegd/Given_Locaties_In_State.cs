namespace AssociationRegistry.Test.Locaties.When_Applying.LocatieWerdToegevoegd;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Locaties_In_State
{
    [Fact]
    public void With_FeitelijkeVereniging_Then_It_Uses_The_Next_NextId()
    {
        var fixture = new Fixture().CustomizeDomain();

        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            Locaties = new[]
            {
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 },
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 2 },
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 3 },
            },
        };

        var state = new VerenigingState()
           .Apply(feitelijkeVerenigingWerdGeregistreerd);

        var locatieWerdToegevoegd = new LocatieWerdToegevoegd(
            Locatie: fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = 4,
            });

        var stateAfterApply = state.Apply(locatieWerdToegevoegd);

        stateAfterApply.Locaties.NextId.Should().Be(5);
    }

    [Fact]
    public void With_VerenigingZonderEigenRechtspersoonlijkheid_Then_It_Uses_The_Next_NextId()
    {
        var fixture = new Fixture().CustomizeDomain();

        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            Locaties = new[]
            {
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 },
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 2 },
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 3 },
            },
        };

        var state = new VerenigingState()
           .Apply(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd);

        var locatieWerdToegevoegd = new LocatieWerdToegevoegd(
            Locatie: fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = 4,
            });

        var stateAfterApply = state.Apply(locatieWerdToegevoegd);

        stateAfterApply.Locaties.NextId.Should().Be(5);
    }
}
