﻿namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using Events;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_LocatieWerdVerwijderd
{

    [Fact]
    public void Then_it_removes_the_locatie()
    {
        var fixture = new Fixture().CustomizeAll();
        var locatieWerdVerwijderd = fixture.Create<TestEvent<LocatieWerdVerwijderd>>();

        var teVerwijderenLocatie = fixture.Create<PubliekVerenigingDetailDocument.Locatie>();
        teVerwijderenLocatie.LocatieId = locatieWerdVerwijderd.Data.Locatie.LocatieId;

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();
        doc.Locaties = doc.Locaties.Append(
            teVerwijderenLocatie).ToArray();

        PubliekVerenigingDetailProjector.Apply(locatieWerdVerwijderd, doc);

        doc.Locaties.Should().NotContain(teVerwijderenLocatie);
    }
}
