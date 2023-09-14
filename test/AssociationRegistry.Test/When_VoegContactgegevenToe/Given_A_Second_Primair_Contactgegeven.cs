namespace AssociationRegistry.Test.When_VoegContactgegevenToe;

using Events;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using Framework.Customizations;
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
                Contactgegevens = new [] { primairContactgegeven },
            }));

        var contactgegeven = fixture.Create<Contactgegeven>() with { IsPrimair = true, Type = primairContactgegeven.Type};

        Assert.Throws<MeerderePrimaireContactgegevensZijnNietToegestaan>(() => vereniging.VoegContactgegevenToe(contactgegeven));
    }
}
