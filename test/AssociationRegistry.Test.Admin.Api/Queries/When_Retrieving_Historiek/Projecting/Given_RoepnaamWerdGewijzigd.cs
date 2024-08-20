<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_RoepnaamWerdGewijzigd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_RoepnaamWerdGewijzigd.cs

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
public class Given_RoepnaamWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var roepnaamWerdGewijzigd = fixture.Create<TestEvent<RoepnaamWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(roepnaamWerdGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"Roepnaam werd gewijzigd naar '{roepnaamWerdGewijzigd.Data.Roepnaam}'.",
                nameof(RoepnaamWerdGewijzigd),
                roepnaamWerdGewijzigd.Data,
                roepnaamWerdGewijzigd.Initiator,
                roepnaamWerdGewijzigd.Tijdstip.ToZuluTime()));
    }
}
