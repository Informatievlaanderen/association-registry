﻿namespace AssociationRegistry.Test.When_VoegVertegenwoordigerToe;

using Events;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_Second_Primaire_Vertegenwoordiger
{
    [Fact]
    public void Then_it_throws()
    {
        var fixture = new Fixture().CustomizeAll();

        var vereniging = new Vereniging();
        var primaireVertegenwoordiger = fixture.Create<Registratiedata.Vertegenwoordiger>() with { IsPrimair = true };
        vereniging.Hydrate(new VerenigingState()
            .Apply(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                Vertegenwoordigers = new [] { primaireVertegenwoordiger },
            }));

        var toeTeVoegenVertegenwoordiger = fixture.Create<Vertegenwoordiger>() with { IsPrimair = true};

        Assert.Throws<MultiplePrimaireVertegenwoordigers>(() => vereniging.VoegVertegenwoordigerToe(toeTeVoegenVertegenwoordiger));
    }
}
