namespace AssociationRegistry.Test.When_VoegLocatieToe;

using AutoFixture;
using Common.AutoFixture;
using Events;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primaire_Locatie
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();
        var primaireLocatie = fixture.Create<Registratiedata.Locatie>() with { IsPrimair = true };

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                               {
                                   Locaties = new[] { primaireLocatie },
                               }));

        var locatie = fixture.Create<Locatie>() with { IsPrimair = true };

        Assert.Throws<MeerderePrimaireLocatiesZijnNietToegestaan>(() => vereniging.VoegLocatieToe(locatie));
    }
}
