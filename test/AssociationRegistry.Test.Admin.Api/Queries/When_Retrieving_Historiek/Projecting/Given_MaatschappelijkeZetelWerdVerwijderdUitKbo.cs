namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Historiek.Projecting;

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
public class Given_MaatschappelijkeZetelWerdVerwijderdUitKbo
{
    [Fact]
    public void Then_it_adds_the_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var maatschappelijkeZetelWerdVerwijderdUitKbo = fixture.Create<TestEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(maatschappelijkeZetelWerdVerwijderdUitKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                Beschrijving: "De locatie met type ‘Maatschappelijke zetel volgens KBO' werd verwijderd uit KBO.",
                nameof(MaatschappelijkeZetelWerdVerwijderdUitKbo),
                maatschappelijkeZetelWerdVerwijderdUitKbo.Data.Locatie,
                maatschappelijkeZetelWerdVerwijderdUitKbo.Initiator,
                maatschappelijkeZetelWerdVerwijderdUitKbo.Tijdstip.ToZuluTime()));
    }
}
