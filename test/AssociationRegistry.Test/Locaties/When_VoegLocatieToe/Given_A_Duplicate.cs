namespace AssociationRegistry.Test.Locaties.When_VoegLocatieToe;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
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
                                                  Locatienaam.Create(locatie.Naam),
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
