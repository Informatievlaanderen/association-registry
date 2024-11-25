namespace AssociationRegistry.Test.When_WijzigLocatie;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_No_Changes
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Does_Not_Emit_Events(VerenigingState givenState, Registratiedata.Locatie duplicateLocatie)
    {
        var vereniging = new VerenigingOfAnyKind();
        vereniging.Hydrate(givenState);

        var adresId = AdresId.Hydrate(Adresbron.Parse(duplicateLocatie.AdresId!.Broncode), duplicateLocatie.AdresId.Bronwaarde);
        var adres = HydrateAdres(duplicateLocatie.Adres!);

        vereniging.WijzigLocatie(duplicateLocatie.LocatieId, duplicateLocatie.Naam, duplicateLocatie.Locatietype,
                                 duplicateLocatie.IsPrimair, adresId, adres);

        vereniging.UncommittedEvents.Should().BeEmpty();
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
            var gewijzigdeLocatie = locatie;

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
            };
        }
    }
}
