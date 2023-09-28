﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VerenigingWerdUitgeschrevenUitPubliekeStroom
{
    [Fact]
    public void Then_it_sets_IsUitgeschrevenUitPubliekeDatastroom_to_true()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var verenigingWerduitgeschrevenUitPubliekeDatastroom =
            fixture.Create<TestEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(verenigingWerduitgeschrevenUitPubliekeDatastroom, doc);

        doc.IsUitgeschrevenUitPubliekeDatastroom.Should().BeTrue();
    }
}
