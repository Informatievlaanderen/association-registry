﻿namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using Events;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_KorteNaamWerdGewijzigd
{
    [Fact]
    public void Then_it_modifies_the_korteNaam()
    {
        var fixture = new Fixture().CustomizeAll();
        var korteNaamWerdGewijzigd = fixture.Create<TestEvent<KorteNaamWerdGewijzigd>>();
        var projector = new PubliekVerenigingDetailProjection();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        projector.Apply(korteNaamWerdGewijzigd, doc);

        doc.KorteNaam.Should().Be(korteNaamWerdGewijzigd.Data.KorteNaam);
        doc.DatumLaatsteAanpassing.Should().Be(korteNaamWerdGewijzigd.Tijdstip.ToBelgianDate());
    }
}
