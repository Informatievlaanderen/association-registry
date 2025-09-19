namespace AssociationRegistry.Test.Locaties.When_VoegLocatieToe;

using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Exceptions;
using Xunit;

public class Given_A_Duplicate_Without_A_Naam
{
    [Fact]
    public void With_FeitelijkeVereniging_Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();

        var locatie = fixture.Create<Registratiedata.Locatie>() with
        {
            Naam = string.Empty,
        };

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                               {
                                   Locaties = [locatie],
                               }));

        Assert.Throws<LocatieIsNietUniek>(() => vereniging.VoegLocatieToe(
                                              Locatie.Create(
                                                  Locatienaam.Empty,
                                                  locatie.IsPrimair,
                                                  locatie.Locatietype,
                                                  AdresId.Create(locatie.AdresId!.Broncode, locatie.AdresId.Bronwaarde),
                                                  Adres.Create(locatie.Adres!.Straatnaam,
                                                               locatie.Adres.Huisnummer,
                                                               locatie.Adres.Busnummer,
                                                               locatie.Adres.Postcode,
                                                               locatie.Adres.Gemeente,
                                                               locatie.Adres.Land))));
    }

    [Fact]
    public void With_VerenigingZonderEigenRechtspersoonlijkheid_Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();

        var locatie = fixture.Create<Registratiedata.Locatie>() with
        {
            Naam = string.Empty,
        };

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
                               {
                                   Locaties = [locatie],
                               }));

        Assert.Throws<LocatieIsNietUniek>(() => vereniging.VoegLocatieToe(
                                              Locatie.Create(
                                                  Locatienaam.Empty,
                                                  locatie.IsPrimair,
                                                  locatie.Locatietype,
                                                  AdresId.Create(locatie.AdresId!.Broncode, locatie.AdresId.Bronwaarde),
                                                  Adres.Create(locatie.Adres!.Straatnaam,
                                                               locatie.Adres.Huisnummer,
                                                               locatie.Adres.Busnummer,
                                                               locatie.Adres.Postcode,
                                                               locatie.Adres.Gemeente,
                                                               locatie.Adres.Land))));
    }
}
