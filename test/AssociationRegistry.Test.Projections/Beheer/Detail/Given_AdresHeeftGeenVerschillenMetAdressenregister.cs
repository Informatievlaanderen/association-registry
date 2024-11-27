namespace AssociationRegistry.Test.Projections.Beheer.Detail;

[Collection(nameof(ProjectionContext))]
public class Given_AdresHeeftGeenVerschillenMetAdressenregister(AdresHeeftGeenVerschillenMetAdressenregisterFixture fixture)
    : IClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterFixture>
{
    [Fact]
    public async Task Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);
}
