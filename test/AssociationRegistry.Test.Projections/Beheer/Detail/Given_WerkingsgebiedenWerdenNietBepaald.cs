namespace AssociationRegistry.Test.Projections.Beheer.Detail;

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
                  .Werkingsgebieden
                  .Should()
                  .BeEmpty();
}
