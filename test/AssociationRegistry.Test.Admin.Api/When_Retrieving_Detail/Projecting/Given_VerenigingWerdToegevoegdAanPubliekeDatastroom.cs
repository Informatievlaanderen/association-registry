﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Projections.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VerenigingWerdToegevoegdAanPubliekeDatastroom
{
    [Fact]
    public void Then_it_sets_IsUitgeschrevenUitPubliekeDatastroom_to_false()
    {
        var fixture = new Fixture().CustomizeAll();
        var verenigingWerdToegevoegdAanPubliekeDatastroom = fixture.Create<TestEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom>>();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(verenigingWerdToegevoegdAanPubliekeDatastroom, doc);

        doc.IsUitgeschrevenUitPubliekeDatastroom.Should().BeFalse();
        doc.DatumLaatsteAanpassing.Should().Be(verenigingWerdToegevoegdAanPubliekeDatastroom.Tijdstip.ToBelgianDate());
        doc.Metadata.Should().BeEquivalentTo(new Metadata(verenigingWerdToegevoegdAanPubliekeDatastroom.Sequence, verenigingWerdToegevoegdAanPubliekeDatastroom.Version));}
}
