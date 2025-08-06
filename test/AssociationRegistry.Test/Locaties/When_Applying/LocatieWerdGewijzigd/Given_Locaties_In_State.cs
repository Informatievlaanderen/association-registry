namespace AssociationRegistry.Test.Locaties.When_Applying.LocatieWerdGewijzigd;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_Locaties_In_State
{
    [Fact]
    public void With_FeitelijkeVerenigign_Then_It_Updates_The_Locatie()
    {
        var fixture = new Fixture().CustomizeDomain();
        var locatieToUpdate = fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

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
                AdresId = locatieToUpdate.AdresId,
            });

        var stateAfterApply = state.Apply(locatieWerdGewijzigd);

        stateAfterApply.Locaties.Should().HaveCount(3);
        var geupdateLocatie = stateAfterApply.Locaties.Should().ContainSingle(l => l.LocatieId == locatieToUpdate.LocatieId).Subject;
        geupdateLocatie.Locatietype.Waarde.Should().Be(locatieWerdGewijzigd.Locatie.Locatietype);
        geupdateLocatie.IsPrimair.Should().Be(locatieWerdGewijzigd.Locatie.IsPrimair);
        geupdateLocatie.Naam.Should().Be(locatieWerdGewijzigd.Locatie.Naam);

        geupdateLocatie.Adres!.Straatnaam.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Straatnaam);
        geupdateLocatie.Adres!.Busnummer.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Busnummer);
        geupdateLocatie.Adres!.Huisnummer.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Huisnummer);
        geupdateLocatie.Adres!.Gemeente.Naam.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Gemeente);
        geupdateLocatie.Adres!.Postcode.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Postcode);
        geupdateLocatie.Adres!.Land.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Land);

        geupdateLocatie.AdresId!.Adresbron.Code.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.AdresId!.Broncode);
        geupdateLocatie.AdresId.Bronwaarde.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.AdresId.Bronwaarde);
        geupdateLocatie.AdresId.Adresbron.Beschrijving.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void With_VerenigingZonderEigenRechtspersoonlijkheid_Then_It_Updates_The_Locatie()
    {
        var fixture = new Fixture().CustomizeDomain();
        var locatieToUpdate = fixture.Create<Registratiedata.Locatie>() with { LocatieId = 1 };

        var verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            Locaties = new[]
            {
                locatieToUpdate,
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 2 },
                fixture.Create<Registratiedata.Locatie>() with { LocatieId = 3 },
            },
        };

        var state = new VerenigingState()
           .Apply(verenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd);

        var locatieWerdGewijzigd = new LocatieWerdGewijzigd(
            Locatie: fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = 1,
                Locatietype = locatieToUpdate.Locatietype,
                IsPrimair = !locatieToUpdate.IsPrimair,
                AdresId = locatieToUpdate.AdresId,
            });

        var stateAfterApply = state.Apply(locatieWerdGewijzigd);

        stateAfterApply.Locaties.Should().HaveCount(3);
        var geupdateLocatie = stateAfterApply.Locaties.Should().ContainSingle(l => l.LocatieId == locatieToUpdate.LocatieId).Subject;
        geupdateLocatie.Locatietype.Waarde.Should().Be(locatieWerdGewijzigd.Locatie.Locatietype);
        geupdateLocatie.IsPrimair.Should().Be(locatieWerdGewijzigd.Locatie.IsPrimair);
        geupdateLocatie.Naam.Should().Be(locatieWerdGewijzigd.Locatie.Naam);

        geupdateLocatie.Adres!.Straatnaam.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Straatnaam);
        geupdateLocatie.Adres!.Busnummer.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Busnummer);
        geupdateLocatie.Adres!.Huisnummer.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Huisnummer);
        geupdateLocatie.Adres!.Gemeente.Naam.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Gemeente);
        geupdateLocatie.Adres!.Postcode.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Postcode);
        geupdateLocatie.Adres!.Land.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.Adres!.Land);

        geupdateLocatie.AdresId!.Adresbron.Code.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.AdresId!.Broncode);
        geupdateLocatie.AdresId.Bronwaarde.Should().BeEquivalentTo(locatieWerdGewijzigd.Locatie.AdresId.Bronwaarde);
        geupdateLocatie.AdresId.Adresbron.Beschrijving.Should().NotBeNullOrWhiteSpace();
    }
}
