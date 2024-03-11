﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Constants;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VerenigingWerdGestopt
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdGestopt = fixture.Create<TestEvent<VerenigingWerdGestopt>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(verenigingWerdGestopt, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"De vereniging werd gestopt met einddatum '{verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly)}'.",
                nameof(VerenigingWerdGestopt),
                verenigingWerdGestopt.Data,
                verenigingWerdGestopt.Initiator,
                verenigingWerdGestopt.Tijdstip.ToZuluTime()));
    }
}

[UnitTest]
public class Given_VerenigingWerdGestoptInKBO
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdGestopt = fixture.Create<TestEvent<VerenigingWerdGestoptInKBO>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(verenigingWerdGestopt, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"De vereniging werd gestopt in KBO met einddatum '{verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly)}'.",
                nameof(VerenigingWerdGestoptInKBO),
                verenigingWerdGestopt.Data,
                verenigingWerdGestopt.Initiator,
                verenigingWerdGestopt.Tijdstip.ToZuluTime()));
    }
}
