namespace AssociationRegistry.Test.Projections.Publiek.Zoeken.Erkenningen;

using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerwijderd(PubliekZoekenScenarioFixture<VzerMetErkenningWerdVerwijderdScenario> fixture)
    : PubliekZoekenScenarioClassFixture<VzerMetErkenningWerdVerwijderdScenario>
{
    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Erkenningen.Should().BeEmpty();
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
