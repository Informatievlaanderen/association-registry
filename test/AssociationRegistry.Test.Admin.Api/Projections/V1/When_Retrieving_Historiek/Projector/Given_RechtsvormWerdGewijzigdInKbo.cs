namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Formats;
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
                rechtsvormWerdGewijzigdInKbo.Tijdstip.FormatAsZuluTime()));
    }
}
