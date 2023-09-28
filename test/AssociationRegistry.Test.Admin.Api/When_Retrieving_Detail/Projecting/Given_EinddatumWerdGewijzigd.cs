﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Constants;
using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_EinddatumWerdGewijzigd
{
    [Fact]
    public void Then_It_Changes_The_Einddatum()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var einddatumWerdGewijzigd = fixture.Create<TestEvent<EinddatumWerdGewijzigd>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(einddatumWerdGewijzigd, doc);

        doc.Einddatum.Should().Be(einddatumWerdGewijzigd.Data.Einddatum.ToString(WellknownFormats.DateOnly));
    }
}
