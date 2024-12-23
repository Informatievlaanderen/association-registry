﻿namespace AssociationRegistry.Test.Locaties.When_VoegLocatieToe;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_CorrespondentieLocatie
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new VerenigingOfAnyKind();
        var primaireLocatie = fixture.Create<Registratiedata.Locatie>() with { Locatietype = Locatietype.Correspondentie };

        vereniging.Hydrate(new VerenigingState()
                              .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
                               {
                                   Locaties = new[] { primaireLocatie },
                               }));

        var locatie = fixture.Create<Locatie>() with { Locatietype = Locatietype.Correspondentie };
        Assert.Throws<MeerdereCorrespondentieLocatiesZijnNietToegestaan>(() => vereniging.VoegLocatieToe(locatie));
    }
}