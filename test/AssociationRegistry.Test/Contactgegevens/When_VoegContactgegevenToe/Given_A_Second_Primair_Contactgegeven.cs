namespace AssociationRegistry.Test.Contactgegevens.When_VoegContactgegevenToe;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Xunit;

public class Given_A_Second_Primair_Contactgegeven
{
    [Fact]
    public void With_FeitelijkeVereniging_Then_it_throws()
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

    [Fact]
    public void With_VerenigingZonderEigenRechtspersoonlijkheid_Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();
        var primairContactgegeven = fixture.Create<Registratiedata.Contactgegeven>() with { IsPrimair = true };

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
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
