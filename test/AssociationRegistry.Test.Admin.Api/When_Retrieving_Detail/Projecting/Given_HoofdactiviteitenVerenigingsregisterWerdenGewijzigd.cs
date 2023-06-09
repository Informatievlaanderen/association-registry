﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;
using Formatters = AssociationRegistry.Admin.Api.Infrastructure.Extensions.Formatters;

[UnitTest]
public class Given_HoofdactiviteitenVerenigingsregisterWerdenGewijzigd
{
    [Fact]
    public void Then_it_replaces_the_hoofdactiviteitenVerenigingsLoket()
    {
        var fixture = new Fixture().CustomizeAll();
        var hoofdactiviteitenVerenigingsloketWerdenGewijzigd = fixture.Create<TestEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(hoofdactiviteitenVerenigingsloketWerdenGewijzigd, doc);

        doc.HoofdactiviteitenVerenigingsloket.Should()
            .BeEquivalentTo(hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket);
        doc.DatumLaatsteAanpassing.Should().Be(Formatters.ToBelgianDate(hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Tijdstip));
        doc.Metadata.Should().BeEquivalentTo(new Metadata(hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Sequence, hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Version));}
}
