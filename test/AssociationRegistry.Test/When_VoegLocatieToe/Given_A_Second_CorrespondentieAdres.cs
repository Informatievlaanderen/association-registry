﻿namespace AssociationRegistry.Test.When_Appending_A_Locatie;

using AutoFixture;
using Events;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_CorrespondentieLocatie
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeAll();

        var vereniging = new Vereniging();
        var primaireLocatie = fixture.Create<Registratiedata.Locatie>() with { Locatietype = Locatietype.Correspondentie };
        vereniging.Hydrate(new VerenigingState()
            .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                Locaties = new [] { primaireLocatie }
            }));

        var locatie = fixture.Create<Locatie>() with { Locatietype = Locatietype.Correspondentie };
        Assert.Throws<DuplicateCorrespondentielocatieProvided>(() => vereniging.VoegLocatieToe(locatie));
    }
}
