namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald(WerkingsgebiedenWerdenNietBepaaldFixture fixture)
    : IClassFixture<WerkingsgebiedenWerdenNietBepaaldFixture>
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
                           Beschrijving: "Werkingsgebieden werden niet bepaald.",
                           nameof(WerkingsgebiedenWerdenNietBepaald),
                           fixture.Scenario.WerkingsgebiedenWerdenNietBepaald,
                           fixture.Context.MetadataInitiator,
                           fixture.Context.MetadataTijdstip)
                   );
}
