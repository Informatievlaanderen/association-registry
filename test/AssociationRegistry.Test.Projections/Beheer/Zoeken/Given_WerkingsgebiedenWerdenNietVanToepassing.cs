namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema;
using Admin.Schema.Search;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using JsonLdContext;
using Scenarios;
using Vereniging;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing : IClassFixture<BeheerZoekClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>>
{
    private readonly BeheerZoekClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> _fixture;

    public Given_WerkingsgebiedenWerdenNietVanToepassing(
        BeheerZoekClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Document_Is_Updated()
    {
        _fixture.Document.Werkingsgebieden
                   .Should().BeEquivalentTo([
                                                new VerenigingZoekDocument.Werkingsgebied
                                                {
                                                    JsonLdMetadata = new JsonLdMetadata(
                                                        JsonLdType.Werkingsgebied.CreateWithIdValues(Werkingsgebied.NietVanToepassing.Code),
                                                        JsonLdType.Werkingsgebied.Type),
                                                    Code = Werkingsgebied.NietVanToepassing.Code,
                                                    Naam = Werkingsgebied.NietVanToepassing.Naam,
                                                },
                                            ]);
    }
}
