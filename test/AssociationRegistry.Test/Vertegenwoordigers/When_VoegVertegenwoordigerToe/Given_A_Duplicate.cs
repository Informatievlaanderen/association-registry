namespace AssociationRegistry.Test.Vertegenwoordigers.When_VoegVertegenwoordigerToe;

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

        var vereniging = new Vereniging();
        var insz = fixture.Create<Insz>();
        var vertegenwoordiger = fixture.Create<Registratiedata.Vertegenwoordiger>() with { Insz = insz };

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                               {
                                   Vertegenwoordigers = new[] { vertegenwoordiger },
                               }));

        var toeTeVoegenVertegenwoordiger = fixture.Create<Vertegenwoordiger>() with { Insz = Insz.Create(vertegenwoordiger.Insz) };
        Assert.Throws<InszMoetUniekZijn>(() => vereniging.VoegVertegenwoordigerToe(toeTeVoegenVertegenwoordiger));
    }
}
