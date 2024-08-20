<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd.cs

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
public class Given_HoofdactiviteitenVerenigingsloketWerdenGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var hoofdactiviteitenVerenigingsloketWerdenGewijzigd =
            fixture.Create<TestEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(hoofdactiviteitenVerenigingsloketWerdenGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                Beschrijving: "Hoofdactiviteiten verenigingsloket werden gewijzigd.",
                nameof(HoofdactiviteitenVerenigingsloketWerdenGewijzigd),
                hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Data,
                hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Initiator,
                hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Tijdstip.ToZuluTime()));
    }
}
