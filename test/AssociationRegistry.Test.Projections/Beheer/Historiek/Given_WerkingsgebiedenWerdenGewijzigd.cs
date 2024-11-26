namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenGewijzigd(WerkingsgebiedenWerdenGewijzigdFixture fixture)
    : IClassFixture<WerkingsgebiedenWerdenGewijzigdFixture>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should()
                  .BeEquivalentTo(
                       new BeheerVerenigingHistoriekGebeurtenis(
                           Beschrijving: "Werkingsgebieden werden gewijzigd.",
                           nameof(WerkingsgebiedenWerdenGewijzigd),
                           fixture.Scenario.WerkingsgebiedenWerdenGewijzigd,
                           fixture.Context.MetadataInitiator,
                           fixture.Context.MetadataTijdstip)
                   );
}
