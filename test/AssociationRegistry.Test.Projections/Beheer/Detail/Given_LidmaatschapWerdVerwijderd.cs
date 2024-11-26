namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdVerwijderd : IClassFixture<BeheerDetailClassFixture<LidmaatschapWerdVerwijderdScenario>>
{
    private readonly BeheerDetailClassFixture<LidmaatschapWerdVerwijderdScenario> _fixture;

    public Given_LidmaatschapWerdVerwijderd(
        BeheerDetailClassFixture<LidmaatschapWerdVerwijderdScenario> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Metadata_Is_Updated()
        => _fixture.Document.Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => _fixture.Document.Lidmaatschappen.Should().BeEmpty();
}
