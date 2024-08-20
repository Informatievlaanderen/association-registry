<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Historiek/Projector/Given_RechtsvormWerdGewijzigdInKbo.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Historiek/Projecting/Given_RechtsvormWerdGewijzigdInKbo.cs

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_RechtsvormWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var rechtsvormWerdGewijzigdInKbo = fixture.Create<TestEvent<RechtsvormWerdGewijzigdInKBO>>();

        var doc = new BeheerVerenigingHistoriekDocument();

        BeheerVerenigingHistoriekProjector.Apply(rechtsvormWerdGewijzigdInKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"In KBO werd de rechtsvorm gewijzigd naar '{Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Naam}'.",
                nameof(RechtsvormWerdGewijzigdInKBO),
                rechtsvormWerdGewijzigdInKbo.Data,
                rechtsvormWerdGewijzigdInKbo.Initiator,
                rechtsvormWerdGewijzigdInKbo.Tijdstip.ToZuluTime()));
    }
}
