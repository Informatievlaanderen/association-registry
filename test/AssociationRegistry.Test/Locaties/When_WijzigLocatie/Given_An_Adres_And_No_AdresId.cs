namespace AssociationRegistry.Test.Locaties.When_WijzigLocatie;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using Xunit;

public class Given_An_Adres_And_No_AdresId
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Emits_LocatieWerdGewijzigdEvent(VerenigingState givenState, Registratiedata.Locatie gewijzigdeLocatie)
    {
        var vereniging = new VerenigingOfAnyKind();
        vereniging.Hydrate(givenState);

        var adres = HydrateAdres(gewijzigdeLocatie.Adres!);

        vereniging.WijzigLocatie(gewijzigdeLocatie.LocatieId, gewijzigdeLocatie.Naam, gewijzigdeLocatie.Locatietype,
                                 gewijzigdeLocatie.IsPrimair, adresId: null, adres);

        vereniging.UncommittedEvents.ToArray().ShouldCompare(
            new IEvent[]
            {
                new LocatieWerdGewijzigd(gewijzigdeLocatie),
            });
    }

    private static Adres HydrateAdres(Registratiedata.Adres gewijzigdeLocatieAdres)
    {
        gewijzigdeLocatieAdres.Deconstruct(
            out var straatnaam,
            out var huisnummer,
            out var busnummer,
            out var postcode,
            out var gemeente,
            out var land);

        return Adres.Hydrate(straatnaam, huisnummer, busnummer, postcode, gemeente, land);
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();
            var locatie = fixture.Create<Registratiedata.Locatie>() with { Locatietype = Locatietype.Activiteiten };

            var gewijzigdeLocatie = locatie with
            {
                Naam = "nieuwe naam",
                Locatietype = Locatietype.Correspondentie,
                IsPrimair = !locatie.IsPrimair,
                AdresId = null,
                Adres = fixture.Create<Registratiedata.Adres>(),
            };

            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                        {
                            Locaties = new[] { locatie },
                        }),
                    gewijzigdeLocatie,
                },
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
                        {
                            Locaties = new[] { locatie },
                        }),
                    gewijzigdeLocatie,
                },
            };
        }
    }
}
