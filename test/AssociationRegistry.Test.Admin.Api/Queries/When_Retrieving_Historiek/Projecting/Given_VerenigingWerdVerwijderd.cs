<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_VerenigingWerdVerwijderd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_VerenigingWerdVerwijderd.cs

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
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
                Beschrijving: "Deze vereniging werd verwijderd.",
                nameof(VerenigingWerdVerwijderd),
                new { verenigingWerdVerwijderd.Data.Reden },
                verenigingWerdVerwijderd.Initiator,
                verenigingWerdVerwijderd.Tijdstip.ToZuluTime()));
    }
}
