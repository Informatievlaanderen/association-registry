namespace AssociationRegistry.Test.Projections.Beheer.Zoeken.Erkenningen;

using Scenario.Erkenningen.Zoeken;

[Collection(nameof(ProjectionContext))]
public class Given_ErkenningWerdVerwijderd(BeheerZoekenScenarioFixture<VzerMetErkenningWerdVerwijderdScenario> fixture)
    : BeheerZoekenScenarioClassFixture<VzerMetErkenningWerdVerwijderdScenario>
{
    [Fact]
    public void Document__Erkenningen_Is_Empty()
    {
        fixture.Result.Erkenningen.Should().BeEmpty();
    }

    [Fact]
    public void IsErkend_Is_False()
    {
        fixture.Result.IsErkend.Should().BeFalse();
    }
}
