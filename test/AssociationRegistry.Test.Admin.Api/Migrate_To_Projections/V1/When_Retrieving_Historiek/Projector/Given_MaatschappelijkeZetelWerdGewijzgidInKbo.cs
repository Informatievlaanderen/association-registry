﻿namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_MaatschappelijkeZetelWerdGewijzgidInKbo
{
    [Fact]
    public void Then_it_adds_the_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var maatschappelijkeZetelWerdGewijzigdInKbo = fixture.Create<TestEvent<MaatschappelijkeZetelWerdGewijzigdInKbo>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(maatschappelijkeZetelWerdGewijzigdInKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                Beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd gewijzigd in KBO.",
                nameof(MaatschappelijkeZetelWerdGewijzigdInKbo),
                maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie,
                maatschappelijkeZetelWerdGewijzigdInKbo.Initiator,
                maatschappelijkeZetelWerdGewijzigdInKbo.Tijdstip.FormatAsZuluTime()));
    }
}
