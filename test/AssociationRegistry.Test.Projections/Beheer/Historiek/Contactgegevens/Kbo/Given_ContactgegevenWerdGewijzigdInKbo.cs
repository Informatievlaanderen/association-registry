namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Contactgegevens.Kbo;

using Admin.Schema.Historiek;
using Events;
using Scenario.Contactgegevens.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdGewijzigdInKbo(
    BeheerHistoriekScenarioFixture<ContactgegevenWerdGewijzigdInKboScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<ContactgegevenWerdGewijzigdInKboScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: $"In KBO werd contactgegeven ‘{fixture.Scenario.ContactgegevenWerdGewijzigdInKbo.TypeVolgensKbo}' gewijzigd.",
                                               nameof(ContactgegevenWerdGewijzigdInKbo),
                                               fixture.Scenario.ContactgegevenWerdGewijzigdInKbo,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
