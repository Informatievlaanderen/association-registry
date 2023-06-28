namespace AssociationRegistry.Test.When_VoegContactgegevenToe;

using Events;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primair_Contactgegeven
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeAll();

        var vereniging = new Vereniging();
        var primairContactgegeven = fixture.Create<Registratiedata.Contactgegeven>() with { IsPrimair = true };
        vereniging.Hydrate(new VerenigingState()
            .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                Contactgegevens = new [] { primairContactgegeven },
            }));

        var contactgegeven = fixture.Create<Contactgegeven>() with { IsPrimair = true, Type = primairContactgegeven.Type};

        Assert.Throws<MultiplePrimairContactgegevens>(() => vereniging.VoegContactgegevenToe(contactgegeven));
    }
}
