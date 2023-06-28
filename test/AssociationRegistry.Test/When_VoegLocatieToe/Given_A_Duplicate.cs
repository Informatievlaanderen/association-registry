namespace AssociationRegistry.Test.When_VoegLocatieToe;

using Events;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeAll();

        var vereniging = new Vereniging();
        var locatie = fixture.Create<Registratiedata.Locatie>();
        vereniging.Hydrate(new VerenigingState()
            .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                Locaties = new [] { locatie },
            }));

        Assert.Throws<DuplicateLocatie>(() => vereniging.VoegLocatieToe(
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
