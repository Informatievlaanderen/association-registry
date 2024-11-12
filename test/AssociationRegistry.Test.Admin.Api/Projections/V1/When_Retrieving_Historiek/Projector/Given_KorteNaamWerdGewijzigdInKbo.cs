namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Formats;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_KorteNaamWerdGewijzigdInKbo
{
    [Fact]
    public void Then_it_adds_a_new_gebeurtenis()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var korteNaamWerdGewijzigdInKbo = fixture.Create<TestEvent<KorteNaamWerdGewijzigdInKbo>>();

        var doc = fixture.Create<BeheerVerenigingHistoriekDocument>();

        BeheerVerenigingHistoriekProjector.Apply(korteNaamWerdGewijzigdInKbo, doc);

        doc.Gebeurtenissen.Should().ContainEquivalentOf(
            new BeheerVerenigingHistoriekGebeurtenis(
                $"In KBO werd de korte naam gewijzigd naar '{korteNaamWerdGewijzigdInKbo.Data.KorteNaam}'.",
                nameof(KorteNaamWerdGewijzigdInKbo),
                korteNaamWerdGewijzigdInKbo.Data,
                korteNaamWerdGewijzigdInKbo.Initiator,
                korteNaamWerdGewijzigdInKbo.Tijdstip.FormatAsZuluTime()));
    }
}
