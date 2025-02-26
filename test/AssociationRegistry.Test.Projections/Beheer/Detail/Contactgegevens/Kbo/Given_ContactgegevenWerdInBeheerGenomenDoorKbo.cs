namespace AssociationRegistry.Test.Projections.Beheer.Detail.Contactgegevens.Kbo;

using Scenario.Contactgegevens.Kbo;
using Vereniging.Bronnen;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdInBeheerGenomenDoorKbo(
    BeheerDetailScenarioFixture<ContactgegevenWerdInBeheerGenomenDoorKboScenario> fixture)
    : BeheerDetailScenarioClassFixture<ContactgegevenWerdInBeheerGenomenDoorKboScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Bron_Is_Updated()
    {
        var contactGegeven = fixture.Result.Contactgegevens.Single(x => x.ContactgegevenId == fixture.Scenario.ContactgegevenWerdInBeheerGenomenDoorKbo.ContactgegevenId);
        contactGegeven.Bron.Should().Be(Bron.KBO);
    }
}
