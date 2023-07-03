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
public class Given_A_NaamVanLocatieWerdGewijzigd
{
    [Fact]
    public void Then_It_Emits_LocatieWerdGewijzigdEvent()
    {
        var fixture = new Fixture().CustomizeAll();

        var vereniging = new Vereniging();
        var locatie = fixture.Create<Registratiedata.Locatie>() with { Locatietype = Locatietype.Activiteiten };
        vereniging.Hydrate(
            new VerenigingState()
                .Apply(
                    fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                    {
                        Locaties = new[] { locatie },
                    }));

        var gewijzigdeLocatie = locatie with
        {
            Naam = "nieuwe naam",
            Locatietype = Locatietype.Correspondentie,
        };

        vereniging.WijzigLocatie(locatie.LocatieId, gewijzigdeLocatie.Naam, gewijzigdeLocatie.Locatietype);

        vereniging.UncommittedEvents.ToArray().ShouldCompare(
            new IEvent[]
            {
                new LocatieWerdGewijzigd(gewijzigdeLocatie),
            });
    }
}
