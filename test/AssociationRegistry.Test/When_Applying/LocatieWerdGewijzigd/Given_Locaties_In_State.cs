namespace AssociationRegistry.Test.When_Applying.LocatieWerdGewijzigd;

using AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
[Category("State")]
public class Given_Locaties_In_State
{
    [Fact]
    public void Then_It_Updates_The_Locatie()
    {
        var fixture = new Fixture().CustomizeDomain();
        var locatieToUpdate = fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1, };
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            Locaties = new[]
            {
                locatieToUpdate,
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 2 },
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 3 },
            },
        };
        var state = new VerenigingState()
            .Apply(feitelijkeVerenigingWerdGeregistreerd);

        var locatieWerdGewijzigd = new LocatieWerdGewijzigd(
            Locatie: fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = 1,
                Locatietype = locatieToUpdate.Locatietype,
                IsPrimair = !locatieToUpdate.IsPrimair,
            });

        var stateAfterApply = state.Apply(locatieWerdGewijzigd);

        stateAfterApply.Locaties.Should().HaveCount(3);
        var geupdateLoactie = stateAfterApply.Locaties.Should().ContainSingle(l => l.LocatieId == locatieToUpdate.LocatieId).Subject;
        geupdateLoactie.Locatietype.Should().Be(locatieWerdGewijzigd.Locatie.Locatietype);
        geupdateLoactie.IsPrimair.Should().Be(locatieWerdGewijzigd.Locatie.IsPrimair);
        geupdateLoactie.Naam.Should().Be(locatieWerdGewijzigd.Locatie.Naam);
        geupdateLoactie.Adres.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres);
        geupdateLoactie.AdresId.Should().Be(locatieWerdGewijzigd.Locatie.AdresId);
    }
}
