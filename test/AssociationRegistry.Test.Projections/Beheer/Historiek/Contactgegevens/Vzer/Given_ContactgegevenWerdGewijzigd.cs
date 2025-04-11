namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Contactgegevens.Vzer;

using Admin.Schema.Historiek;
using Events;
using Scenario.Contactgegevens.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdGewijzigd(
    BeheerHistoriekScenarioFixture<ContactgegevenWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<ContactgegevenWerdGewijzigdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Gebeurtenissen.Last()
                  .Should().BeEquivalentTo(new BeheerVerenigingHistoriekGebeurtenis(
                                               Beschrijving: $"{fixture.Scenario.ContactgegevenWerdGewijzigd.Contactgegeventype} '{fixture.Scenario.ContactgegevenWerdGewijzigd.Waarde}' werd gewijzigd.",
                                               nameof(ContactgegevenWerdGewijzigd),
                                               fixture.Scenario.ContactgegevenWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
