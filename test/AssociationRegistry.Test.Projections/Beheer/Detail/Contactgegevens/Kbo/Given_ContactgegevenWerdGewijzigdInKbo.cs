namespace AssociationRegistry.Test.Projections.Beheer.Detail.Contactgegevens.Kbo;

using AssociationRegistry.Test.Projections.Scenario.Contactgegevens;
using Scenario.Contactgegevens.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdGewijzigdInKbo(
    BeheerDetailScenarioFixture<ContactgegevenWerdGewijzigdInKboScenario> fixture)
    : BeheerDetailScenarioClassFixture<ContactgegevenWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Waarde_Is_Updated()
    {
        var gewijzgidContactGegeven = fixture.Result.Contactgegevens.Single(x => x.ContactgegevenId == fixture.Scenario.ContactgegevenWerdGewijzigdInKbo.ContactgegevenId);
        gewijzgidContactGegeven.Waarde.Should().Be(fixture.Scenario.ContactgegevenWerdGewijzigdInKbo.Waarde);
    }
}
