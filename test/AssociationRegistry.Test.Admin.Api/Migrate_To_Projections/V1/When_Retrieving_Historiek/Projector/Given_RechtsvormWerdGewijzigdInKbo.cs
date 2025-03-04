namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Historiek.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Historiek;
using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Common.AutoFixture;
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
                rechtsvormWerdGewijzigdInKbo.Tijdstip.FormatAsZuluTime()));
    }
}
