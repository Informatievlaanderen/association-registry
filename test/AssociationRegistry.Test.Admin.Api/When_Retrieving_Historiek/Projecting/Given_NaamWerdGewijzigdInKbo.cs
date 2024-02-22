namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Constants;
using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AutoFixture;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
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
                korteNaamWerdGewijzigdInKbo.Tijdstip.ToZuluTime()));
    }
}
