namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_AdresHeeftGeenVerschillenMetAdressenregister : IClassFixture<DetailClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario>>
{
    private readonly DetailClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario> _fixture;

    public Given_AdresHeeftGeenVerschillenMetAdressenregister(
        DetailClassFixture<AdresHeeftGeenVerschillenMetAdressenregisterScenario> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public Task Metadata_Is_Updated()
        => Task.FromResult(_fixture.Document.Metadata.Version.Should().Be(2));
}
