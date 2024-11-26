namespace AssociationRegistry.Test.Projections.Beheer.Historiek;

using Admin.Schema.Historiek;
using Events;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Scenarios;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_WerkingsgebiedenWerdenNietVanToepassing : IClassFixture<HistoriekClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario>>
{
    private readonly ProjectionContext _context;
    private readonly HistoriekClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> _fixture;

    public Given_WerkingsgebiedenWerdenNietVanToepassing(
        ProjectionContext context,
        HistoriekClassFixture<WerkingsgebiedenWerdenNietVanToepassingScenario> fixture)
    {
        _context = context;
        _fixture = fixture;
    }

    [Fact]
    public void Metadata_Is_Updated()
        => _fixture.Document.Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => _fixture.Document.Gebeurtenissen.Last()
                   .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                                Beschrijving: "Werkingsgebieden werden niet van toepassing.",
                                                nameof(WerkingsgebiedenWerdenNietVanToepassing),
                                                _fixture.Scenario.WerkingsgebiedenWerdenNietVanToepassing,
                                                _context.MetadataInitiator,
                                                _context.MetadataTijdstip));
}
