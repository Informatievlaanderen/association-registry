﻿namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using Events;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Formats;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_WerkingsgebiedenWerdenGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var werkingsgebiedenWerdenGewijzigd =
            fixture.Create<TestEvent<WerkingsgebiedenWerdenGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(werkingsgebiedenWerdenGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                Beschrijving: "Werkingsgebieden werden gewijzigd.",
                nameof(WerkingsgebiedenWerdenGewijzigd),
                werkingsgebiedenWerdenGewijzigd.Data,
                werkingsgebiedenWerdenGewijzigd.Initiator,
                werkingsgebiedenWerdenGewijzigd.Tijdstip.FormatAsZuluTime()));
    }
}
