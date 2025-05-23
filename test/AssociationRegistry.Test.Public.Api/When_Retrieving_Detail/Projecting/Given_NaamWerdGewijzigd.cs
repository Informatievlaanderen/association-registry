﻿namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

public class Given_NaamWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_naam()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var naamWerdGewijzigd = fixture.Create<TestEvent<NaamWerdGewijzigd>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(naamWerdGewijzigd, doc);

        doc.Naam.Should().Be(naamWerdGewijzigd.Data.Naam);
    }
}
