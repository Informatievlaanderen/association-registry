namespace AssociationRegistry.Test.When_WijzigLocatie;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Duplicate
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Emits_LocatieWerdGewijzigdEvent(VerenigingState givenState, Registratiedata.Locatie duplicateLocatie)
    {
        var vereniging = new Vereniging();
        vereniging.Hydrate(givenState);

        var adresId = AdresId.Hydrate(Adresbron.Parse(duplicateLocatie.AdresId!.Broncode), duplicateLocatie.AdresId.Bronwaarde);
        var adres = HydrateAdres(duplicateLocatie.Adres!);

        var wijzigLocatie = () => vereniging.WijzigLocatie(duplicateLocatie.LocatieId, duplicateLocatie.Naam, duplicateLocatie.Locatietype, duplicateLocatie.IsPrimair, adresId, adres);
        wijzigLocatie.Should().Throw<DuplicateLocatie>();
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
            var fixture = new Fixture().CustomizeAll();
            var locatie = fixture.Create<Registratiedata.Locatie>() with { Locatietype = Locatietype.Activiteiten };
            var gewijzigdeLocatie = locatie;

            return new List<object[]>
            {
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<AfdelingWerdGeregistreerd>() with
                        {
                            Locaties = new[] { locatie },
                        }),
                    gewijzigdeLocatie,
                },
                new object[]
                {
                    new VerenigingState().Apply(
                        fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                        {
                            Locaties = new[] { locatie },
                        }),
                    gewijzigdeLocatie,
                },
            };
        }
    }
}
