namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Events;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietBepaald : IClassFixture<HistoriekClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario>>
{
    private readonly ProjectionContext _context;
    private readonly HistoriekClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario> _fixture;

    public Given_WerkingsgebiedenWerdenNietBepaald(
        ProjectionContext context,
        HistoriekClassFixture<WerkingsgebiedenWerdenNietBepaaldScenario> fixture)
    {
        _context = context;
        _fixture = fixture;
    }

    [Fact]
    public void Metadata_Is_Updated()
    {
        _fixture.Document.Metadata.Version.Should().Be(3);
    }

    [Fact]
    public void Document_Is_Updated()
        => _fixture.Document.Gebeurtenissen.Last()
                   .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                                Beschrijving: "Werkingsgebieden werden niet bepaald.",
                                                nameof(WerkingsgebiedenWerdenNietBepaald),
                                                _fixture.Scenario.WerkingsgebiedenWerdenNietBepaald,
                                                _context.MetadataInitiator,
                                                _context.MetadataTijdstip));
}
