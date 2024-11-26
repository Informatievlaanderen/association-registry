namespace AssociationRegistry.Test.Projections.Beheer.Zoeken;

using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald : IClassFixture<BeheerZoekClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario>>
{
    private readonly ProjectionContext _context;
    private readonly BeheerZoekClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario> _fixture;

    public Given_WerkingsgebiedenWerdenNietBepaald(
        ProjectionContext context,
        BeheerZoekClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario> fixture)
    {
        _context = context;
        _fixture = fixture;
    }

    [Fact]
    public void Document_Is_Updated()
    {
        _fixture.Document.Werkingsgebieden
                   .Should().BeEmpty();
    }
}
