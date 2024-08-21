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
public class Given_MaatschappelijkeZetelVolgensKBOWerdGewijzigd
{
    [Fact]
    public void Then_it_adds_the_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var maatschappelijkeZetelVolgensKboWerdGewijzigd = fixture.Create<TestEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(maatschappelijkeZetelVolgensKboWerdGewijzigd, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                Beschrijving: "Maatschappelijke zetel volgens KBO werd gewijzigd.",
                nameof(MaatschappelijkeZetelVolgensKBOWerdGewijzigd),
                maatschappelijkeZetelVolgensKboWerdGewijzigd.Data,
                maatschappelijkeZetelVolgensKboWerdGewijzigd.Initiator,
                maatschappelijkeZetelVolgensKboWerdGewijzigd.Tijdstip.ToZuluTime()));
    }
}
