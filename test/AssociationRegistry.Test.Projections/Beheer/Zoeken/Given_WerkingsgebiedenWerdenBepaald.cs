namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using Admin.Schema;
using Admin.Schema.Search;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using JsonLdContext;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenBepaald : IClassFixture<BeheerZoekClassFixture<WerkingsgebiedenWerdenBepaaldScenario>>
{
    private readonly BeheerZoekClassFixture<WerkingsgebiedenWerdenBepaaldScenario> _fixture;

    public Given_WerkingsgebiedenWerdenBepaald(
        BeheerZoekClassFixture<WerkingsgebiedenWerdenBepaaldScenario> fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Document_Is_Updated()
    {
        _fixture.Document.Werkingsgebieden
                   .Should().BeEquivalentTo(_fixture.Scenario.WerkingsgebiedenWerdenBepaald.Werkingsgebieden.Select(
                                                s => new VerenigingZoekDocument.Werkingsgebied
                                                {
                                                    JsonLdMetadata = new JsonLdMetadata(
                                                        JsonLdType.Werkingsgebied.CreateWithIdValues(s.Code),
                                                        JsonLdType.Werkingsgebied.Type),
                                                    Code = s.Code,
                                                    Naam = s.Naam,
                                                }));
    }
}
