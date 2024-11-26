namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Events;
using FluentAssertions;
using Framework;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing(WerkingsgebiedenWerdenNietVanToepassingFixture fixture)
    : IClassFixture<WerkingsgebiedenWerdenNietVanToepassingFixture>
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
                           Beschrijving: "Werkingsgebieden werden niet van toepassing.",
                           nameof(WerkingsgebiedenWerdenNietVanToepassing),
                           fixture.Scenario.WerkingsgebiedenWerdenNietVanToepassing,
                           fixture.Context.MetadataInitiator,
                           fixture.Context.MetadataTijdstip)
                   );
}
