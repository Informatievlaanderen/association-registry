namespace AssociationRegistry.Test.Projections.Beheer.Detail.Contactgegevens.Vzer;

using Scenario.Contactgegevens.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdGewijzigd(
    BeheerDetailScenarioFixture<ContactgegevenWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<ContactgegevenWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Waarde_Is_Updated()
    {
        var gewijzgidContactGegeven = fixture.Result.Contactgegevens.Single(x => x.ContactgegevenId == fixture.Scenario.ContactgegevenWerdGewijzigd.ContactgegevenId);
        gewijzgidContactGegeven.Waarde.Should().Be(fixture.Scenario.ContactgegevenWerdGewijzigd.Waarde);
        gewijzgidContactGegeven.Beschrijving.Should().Be(fixture.Scenario.ContactgegevenWerdGewijzigd.Beschrijving);
        gewijzgidContactGegeven.IsPrimair.Should().Be(fixture.Scenario.ContactgegevenWerdGewijzigd.IsPrimair);
    }
}
