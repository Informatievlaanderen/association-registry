namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_AdresHeeftGeenVerschillenMetAdressenregister : IClassFixture<BeheerDetailClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario>>
{
    private readonly BeheerDetailClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario> _fixture;

    public Given_AdresHeeftGeenVerschillenMetAdressenregister(
        BeheerDetailClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Metadata_Is_Updated()
        => _fixture.Document.Metadata.Version.Should().Be(2);
}
