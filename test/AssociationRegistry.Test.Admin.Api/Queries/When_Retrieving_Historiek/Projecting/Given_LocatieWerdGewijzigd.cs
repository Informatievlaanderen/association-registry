<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_LocatieWerdGewijzigd.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_LocatieWerdGewijzigd.cs

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
public class Given_LocatieWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_the_locatie_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieWerdGewijzigd = fixture.Create<TestEvent<LocatieWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(locatieWerdGewijzigd, doc);

        var naam = string.IsNullOrEmpty(locatieWerdGewijzigd.Data.Locatie.Naam)
            ? string.Empty
            : $"'{locatieWerdGewijzigd.Data.Locatie.Naam}' ";

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"'{locatieWerdGewijzigd.Data.Locatie.Locatietype}' locatie {naam}werd gewijzigd.",
                nameof(LocatieWerdGewijzigd),
                locatieWerdGewijzigd.Data.Locatie,
                locatieWerdGewijzigd.Initiator,
                locatieWerdGewijzigd.Tijdstip.ToZuluTime()));
    }
}
