namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald : IClassFixture<BeheerDetailClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario>>
{
    private readonly BeheerDetailClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario> _fixture;

    public Given_WerkingsgebiedenWerdenNietBepaald(
        BeheerDetailClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Metadata_Is_Updated()
        => _fixture.Document.Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => _fixture.Document.Werkingsgebieden
                   .Should()
                   .BeEmpty();
}
