<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_NaamWerdGewijzigdInKbo.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_NaamWerdGewijzigdInKbo.cs

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
public class Given_NaamWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var naamWerdGewijzigdInKbo = fixture.Create<TestEvent<NaamWerdGewijzigdInKbo>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(naamWerdGewijzigdInKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"In KBO werd de naam gewijzigd naar '{naamWerdGewijzigdInKbo.Data.Naam}'.",
                nameof(NaamWerdGewijzigdInKbo),
                naamWerdGewijzigdInKbo.Data,
                naamWerdGewijzigdInKbo.Initiator,
                naamWerdGewijzigdInKbo.Tijdstip.ToZuluTime()));
    }
}
