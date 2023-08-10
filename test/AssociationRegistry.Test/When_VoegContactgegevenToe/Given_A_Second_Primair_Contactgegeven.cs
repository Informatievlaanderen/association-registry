﻿namespace AssociationRegistry.Test.When_VoegContactgegevenToe;

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

        var vereniging = new Vereniging();
        var primairContactgegeven = fixture.Create<Registratiedata.Contactgegeven>() with { IsPrimair = true };
        vereniging.Hydrate(
            new VerenigingState()
                .Apply(
                    fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                    {
                        Contactgegevens = new[] { primairContactgegeven },
                    }));

        var contactgegeven = Contactgegeven.Hydrate(primairContactgegeven.ContactgegevenId + 1, primairContactgegeven.Type, primairContactgegeven.Waarde, fixture.Create<string>(), true);

        Assert.Throws<MultiplePrimairContactgegevens>(() => vereniging.VoegContactgegevenToe(contactgegeven));
    }
}
