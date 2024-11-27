namespace AssociationRegistry.Test.Projections.Publiek.Detail;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald(WerkingsgebiedenWerdenNietBepaaldFixture fixture)
    : IClassFixture<WerkingsgebiedenWerdenNietBepaaldFixture>
{
    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Werkingsgebieden
                  .Should()
                  .BeEmpty();
}
