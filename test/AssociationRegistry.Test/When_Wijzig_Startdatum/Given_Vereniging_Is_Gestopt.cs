﻿namespace AssociationRegistry.Test.When_Wijzig_Startdatum;

using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;

public class Given_Vereniging_Is_Gestopt
{
    [Fact]
    public void Then_It_Throws_If_Startdatum_Is_After_Einddatum()
    {
        var fixture = new Fixture().CustomizeDomain();

        var vereniging = new Vereniging();

        var einddatum = fixture.Create<Datum>();
        var feitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        var verenigingWerdGestopt = VerenigingWerdGestopt.With(einddatum);

        vereniging.Hydrate(
            new VerenigingState()
               .Apply(feitelijkeVerenigingWerdGeregistreerd)
               .Apply(verenigingWerdGestopt));

        var clock = new ClockStub(einddatum.Value.AddDays(100));

        var startdatum = Datum.Hydrate(einddatum.Value.AddDays(1));
        var wijzigStartdatum = () => vereniging.WijzigStartdatum(startdatum, clock);

        wijzigStartdatum.Should().Throw<StartdatumIsAfterEinddatum>();
    }

    [Fact]
    public void Then_It_Has_Events_If_Startdatum_Is_Before_Einddatum()
    {
    }

    [Fact]
    public void Then_It_Has_Events_If_Startdatum_Is_Equal_To_Einddatum()
    {
    }
}
