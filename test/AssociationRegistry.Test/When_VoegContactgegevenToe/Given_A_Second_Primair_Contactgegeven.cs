namespace AssociationRegistry.Test.When_VoegContactgegevenToe;

using AutoFixture;
using Common.AutoFixture;
using Events;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primair_Contactgegeven
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();
        var primairContactgegeven = fixture.Create<Registratiedata.Contactgegeven>() with { IsPrimair = true };

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                               {
                                   Contactgegevens = new[] { primairContactgegeven },
                               }));

        var contactgegeven = fixture.Create<Contactgegeven>() with
        {
            IsPrimair = true, Contactgegeventype = primairContactgegeven.Contactgegeventype,
        };

        Assert.Throws<MeerderePrimaireContactgegevensZijnNietToegestaan>(() => vereniging.VoegContactgegevenToe(contactgegeven));
    }
}
