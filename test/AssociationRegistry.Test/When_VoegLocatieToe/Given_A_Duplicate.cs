namespace AssociationRegistry.Test.When_VoegLocatieToe;

using AutoFixture;
using Events;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();
        var locatie = fixture.Create<Registratiedata.Locatie>();

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                               {
                                   Locaties = new[] { locatie },
                               }));

        Assert.Throws<LocatieIsNietUniek>(() => vereniging.VoegLocatieToe(
                                              Locatie.Create(
                                                  locatie.Naam,
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
    public void With_Null_Name_And_Existing_Empty_Locatie_Name_Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();

        var locatie = fixture.Create<Registratiedata.Locatie>()
            with
            {
                Naam = ""
            };

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                               {
                                   Locaties = new[] { locatie with { Naam = null } },
                               }));

        Assert.Throws<LocatieIsNietUniek>(() => vereniging.VoegLocatieToe(
                                              Locatie.Create(
                                                  locatie.Naam,
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
