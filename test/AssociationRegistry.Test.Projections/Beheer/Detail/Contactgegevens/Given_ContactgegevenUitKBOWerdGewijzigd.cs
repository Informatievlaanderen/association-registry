namespace AssociationRegistry.Test.Projections.Beheer.Detail.Contactgegevens;

using Scenario.Contactgegevens;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenUitKBOWerdGewijzigd(
    BeheerDetailScenarioFixture<ContactgegevenUitKBOWerdGewijzigdScenario> fixture)
    : BeheerDetailScenarioClassFixture<ContactgegevenUitKBOWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void ContactGegeven_Werd_Gewijzigd()
    {
        var gewijzgidContactGegeven = fixture.Result.Contactgegevens.Single(x => x.ContactgegevenId == fixture.Scenario.ContactgegevenUitKBOWerdGewijzigd.ContactgegevenId);
        gewijzgidContactGegeven.Beschrijving.Should().Be(fixture.Scenario.ContactgegevenUitKBOWerdGewijzigd.Beschrijving);
        gewijzgidContactGegeven.IsPrimair.Should().Be(fixture.Scenario.ContactgegevenUitKBOWerdGewijzigd.IsPrimair);
    }
}
