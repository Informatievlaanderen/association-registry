﻿namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;
using WellknownFormats = AssociationRegistry.Admin.ProjectionHost.Constants.WellknownFormats;

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
                verenigingWerdGestopt.Tijdstip.FormatAsZuluTime()));
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
                verenigingWerdGestopt.Tijdstip.FormatAsZuluTime()));
    }
}
