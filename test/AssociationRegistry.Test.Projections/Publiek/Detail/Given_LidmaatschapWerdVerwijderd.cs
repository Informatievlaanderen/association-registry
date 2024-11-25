namespace AssociationRegistry.Test.Projections.Publiek.Detail;

using FluentAssertions;
using Framework;
using Marten;
using Public.Schema.Detail;
using ScenarioClassFixtures;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Given_LidmaatschapWerdVerwijderd : IClassFixture<LidmaatschapWerdVerwijderdScenario>
{
    private readonly ProjectionContext _context;
    private readonly LidmaatschapWerdVerwijderdScenario _scenario;

    public Given_LidmaatschapWerdVerwijderd(
        ProjectionContext context,
        LidmaatschapWerdVerwijderdScenario scenario)
    {
        _context = context;
        _scenario = scenario;
    }

    [Fact]
    public async Task Document_Is_Updated()
    {
        var document =
            await _context
                 .Session
                 .Query<PubliekVerenigingDetailDocument>()
                 .Where(w => w.VCode == _scenario.LidmaatschapWerdVerwijderd.VCode)
                 .SingleAsync();

        document.Lidmaatschappen.Should().BeEmpty();
    }
}
