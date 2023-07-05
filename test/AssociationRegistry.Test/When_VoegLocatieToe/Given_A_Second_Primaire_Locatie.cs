namespace AssociationRegistry.Test.When_VoegLocatieToe;

using Events;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using Framework.Customizations;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primaire_Locatie
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();
        var primaireLocatie = fixture.Create<Registratiedata.Locatie>() with { IsPrimair = true };
        vereniging.Hydrate(new VerenigingState()
            .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                Locaties = new [] { primaireLocatie },
            }));

        var locatie = fixture.Create<Locatie>() with { IsPrimair = true };

        Assert.Throws<MultiplePrimaireLocaties>(() => vereniging.VoegLocatieToe(locatie));
    }
}
