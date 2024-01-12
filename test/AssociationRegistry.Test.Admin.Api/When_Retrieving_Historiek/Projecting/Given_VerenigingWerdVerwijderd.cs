﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

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
public class Given_VerenigingWerdVerwijderd
{
    [Fact]
    public void Then_it_adds_the_verwijderd_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var verenigingWerdVerwijderd = fixture.Create<TestEvent<VerenigingWerdVerwijderd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(verenigingWerdVerwijderd, doc);

        doc.Gebeurtenissen.Should().HaveCount(1);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                "Deze vereniging werd verwijderd.",
                nameof(VerenigingWerdVerwijderd),
                verenigingWerdVerwijderd.Data,
                verenigingWerdVerwijderd.Initiator,
                verenigingWerdVerwijderd.Tijdstip.ToZuluTime()));
    }
}
