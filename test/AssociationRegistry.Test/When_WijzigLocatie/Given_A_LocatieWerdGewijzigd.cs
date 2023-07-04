namespace AssociationRegistry.Test.When_WijzigLocatie;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Framework;
using KellermanSoftware.CompareNetObjects;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_LocatieWerdGewijzigd
{

    [Theory]
    [MemberData(nameof(Data))]
    public void Then_It_Emits_LocatieWerdGewijzigdEvent(VerenigingState givenState, Registratiedata.Locatie gewijzigdeLocatie)
    {
        var vereniging = new Vereniging();
        vereniging.Hydrate(givenState);

        vereniging.WijzigLocatie(gewijzigdeLocatie.LocatieId, gewijzigdeLocatie.Naam, gewijzigdeLocatie.Locatietype);

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
            var fixture = new Fixture().CustomizeAll();
            var locatie = fixture.Create<Registratiedata.Locatie>() with { Locatietype = Locatietype.Activiteiten };
            var gewijzigdeLocatie = locatie with
            {
                Naam = "nieuwe naam",
                Locatietype = Locatietype.Correspondentie,
            };

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
