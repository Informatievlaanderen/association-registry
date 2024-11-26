namespace AssociationRegistry.Test.Projections.Beheer.Detail;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using JsonLdContext;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing : IClassFixture<BeheerDetailClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>>
{
    private readonly BeheerDetailClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> _fixture;

    public Given_WerkingsgebiedenWerdenNietVanToepassing(
        BeheerDetailClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> fixture)
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
                   .BeEquivalentTo(
                        [
                            new Werkingsgebied
                            {
                                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                    JsonLdType.Werkingsgebied,
                                    Vereniging.Werkingsgebied.NietVanToepassing.Code),
                                Code = Vereniging.Werkingsgebied.NietVanToepassing.Code,
                                Naam = Vereniging.Werkingsgebied.NietVanToepassing.Naam,
                            },
                        ]
                    );
}
