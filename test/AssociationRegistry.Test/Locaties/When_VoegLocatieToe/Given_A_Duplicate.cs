namespace AssociationRegistry.Test.Locaties.When_VoegLocatieToe;

using Events;
using AssociationRegistry.Test.Common.AutoFixture;
using Vereniging;
using AutoFixture;
using AutoFixture.Kernel;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Exceptions;
using Xunit;

public class Given_A_Duplicate
{
    [Theory]
    [InlineData(typeof(FeitelijkeVerenigingWerdGeregistreerd))]
    [InlineData(typeof(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd))]
    public void Then_it_throws(Type verenigingType)
    {
        var fixture = new Fixture().CustomizeDomain();
        var context = new SpecimenContext(fixture);

        var vereniging = new VerenigingOfAnyKind();
        var verenigingWerdGeregistreerd = (IVerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd)context.Resolve(verenigingType);
        var locatie = verenigingWerdGeregistreerd.Locaties.First();


        vereniging.Hydrate(new VerenigingState()
                              .Apply((dynamic)verenigingWerdGeregistreerd));

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
