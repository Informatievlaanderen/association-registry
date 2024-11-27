namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Events;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenBepaald(WerkingsgebiedenWerdenBepaaldFixture fixture)
    : IClassFixture<WerkingsgebiedenWerdenBepaaldFixture>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: "Werkingsgebieden werden bepaald.",
                                               nameof(WerkingsgebiedenWerdenBepaald),
                                               fixture.Scenario.WerkingsgebiedenWerdenBepaald,
                                               fixture.Context.MetadataInitiator,
                                               fixture.Context.MetadataTijdstip));
}
