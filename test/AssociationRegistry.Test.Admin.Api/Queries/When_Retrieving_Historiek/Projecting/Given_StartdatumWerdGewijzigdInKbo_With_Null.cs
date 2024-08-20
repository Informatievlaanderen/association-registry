<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_StartdatumWerdGewijzigdInKbo_With_Null.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_StartdatumWerdGewijzigdInKbo_With_Null.cs

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
public class Given_StartdatumWerdGewijzigdInKbo_With_Null
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis_with_null()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var startdatumWerdGewijzigd = fixture.Create<TestEvent<StartdatumWerdGewijzigdInKbo>>();

        startdatumWerdGewijzigd.Data = new StartdatumWerdGewijzigdInKbo(Startdatum: null);

        var doc = new BeheerVerenigingHistoriekDocument();

        BeheerVerenigingHistoriekProjector.Apply(startdatumWerdGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                Beschrijving: "In KBO werd de startdatum verwijderd.",
                nameof(StartdatumWerdGewijzigdInKbo),
                startdatumWerdGewijzigd.Data,
                startdatumWerdGewijzigd.Initiator,
                startdatumWerdGewijzigd.Tijdstip.ToZuluTime()));
    }
}
