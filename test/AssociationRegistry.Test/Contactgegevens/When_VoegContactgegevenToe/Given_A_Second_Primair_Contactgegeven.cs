﻿namespace AssociationRegistry.Test.Contactgegevens.When_VoegContactgegevenToe;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
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