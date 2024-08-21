namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
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
                maatschappelijkeZetelWerdGewijzigdInKbo.Tijdstip.ToZuluTime()));
    }
}
