namespace AssociationRegistry.Test.Projections.Beheer.Detail.Contactgegevens.Vzer;

using Scenario.Contactgegevens.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdVerwijderd(
    BeheerDetailScenarioFixture<ContactgegevenWerdVerwijderdScenario> fixture)
    : BeheerDetailScenarioClassFixture<ContactgegevenWerdVerwijderdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        var contactGegeven = fixture.Result.Contactgegevens.SingleOrDefault(x => x.ContactgegevenId == fixture.Scenario.ContactgegevenWerdVerwijderd.ContactgegevenId);
        contactGegeven.Should().BeNull();
    }
}
