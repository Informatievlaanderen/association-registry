namespace AssociationRegistry.Test.Locaties.When_WijzigMaatschappelijkeZetel;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using KellermanSoftware.CompareNetObjects;
using System.Linq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_All_Fields
{
    [Fact]
    public void Then_It_Emits_MaatschappelijkeZetelVolgensKBOWerdGewijzigd()
    {
        var fixture = new Fixture().CustomizeDomain();

        var maatschappelijkeZetel = fixture.Create<Registratiedata.Locatie>() with
        {
            LocatieId = 1,
            Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo,
        };

        var vereniging = new VerenigingMetRechtspersoonlijkheid();

        vereniging.Hydrate(new VerenigingState().Apply(
                                                     fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                                                .Apply(new MaatschappelijkeZetelWerdOvergenomenUitKbo(maatschappelijkeZetel)));

        var naam = fixture.Create<string>();
        var isPrimair = fixture.Create<bool>();
        vereniging.WijzigMaatschappelijkeZetel(maatschappelijkeZetel.LocatieId, naam, isPrimair);

        vereniging.UncommittedEvents.ToArray().ShouldCompare(
            new IEvent[]
            {
                new MaatschappelijkeZetelVolgensKBOWerdGewijzigd(maatschappelijkeZetel.LocatieId, naam,
                                                                 isPrimair),
            });
    }
}
