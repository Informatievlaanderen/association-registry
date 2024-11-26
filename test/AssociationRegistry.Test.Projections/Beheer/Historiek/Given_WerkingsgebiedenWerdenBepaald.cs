namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Events;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenBepaald : IClassFixture<HistoriekClassFixture<WerkingsgebiedenWerdenBepaaldScenario>>
{
    private readonly ProjectionContext _context;
    private readonly HistoriekClassFixture<WerkingsgebiedenWerdenBepaaldScenario> _fixture;

    public Given_WerkingsgebiedenWerdenBepaald(
        ProjectionContext context,
        HistoriekClassFixture<WerkingsgebiedenWerdenBepaaldScenario> fixture)
    {
        _context = context;
        _fixture = fixture;
    }

    [Fact]
    public void Metadata_Is_Updated()
        => _fixture.Document.Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        _fixture.Document.Gebeurtenissen.Last()
                .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                             Beschrijving: "Werkingsgebieden werden bepaald.",
                                             nameof(WerkingsgebiedenWerdenBepaald),
                                             _fixture.Scenario.WerkingsgebiedenWerdenBepaald,
                                             _context.MetadataInitiator,
                                             _context.MetadataTijdstip));
    }
}
