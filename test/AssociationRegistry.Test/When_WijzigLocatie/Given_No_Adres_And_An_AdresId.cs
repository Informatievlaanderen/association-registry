namespace AssociationRegistry.Test.When_WijzigLocatie;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Events;
using Framework.Customizations;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_No_Adres_And_An_AdresId
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Emits_LocatieWerdGewijzigdEvent(VerenigingState givenState, Registratiedata.Locatie gewijzigdeLocatie)
    {
        var vereniging = new VerenigingOfAnyKind();
        vereniging.Hydrate(givenState);

        var adresId = AdresId.Hydrate(Adresbron.Parse(gewijzigdeLocatie.AdresId!.Broncode), gewijzigdeLocatie.AdresId.Bronwaarde);

        vereniging.WijzigLocatie(gewijzigdeLocatie.LocatieId, gewijzigdeLocatie.Naam, gewijzigdeLocatie.Locatietype,
                                 gewijzigdeLocatie.IsPrimair, adresId, adres: null);

        vereniging.UncommittedEvents.ToArray().ShouldCompare(
            new IEvent[]
            {
                new LocatieWerdGewijzigd(gewijzigdeLocatie),
            });
    }

    public static IEnumerable<object[]> Data
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();
            var locatie = fixture.Create<Registratiedata.Locatie>() with { Locatietype = Locatietype.Activiteiten };

            var gewijzigdeLocatie = locatie with
            {
                AdresId = fixture.Create<Registratiedata.AdresId>(),
                Adres = null,
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
            };
        }
    }
}
