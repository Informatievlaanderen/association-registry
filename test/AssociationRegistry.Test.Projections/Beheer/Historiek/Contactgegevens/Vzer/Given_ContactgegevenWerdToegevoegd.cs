namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Contactgegevens.Vzer;

using Admin.Schema.Historiek;
using Events;
using Scenario.Contactgegevens.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdToegevoegd(
    BeheerHistoriekScenarioFixture<ContactgegevenWerdToegevoegdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<ContactgegevenWerdToegevoegdScenario>
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
                                               Beschrijving: $"{fixture.Scenario.ContactgegevenWerdToegevoegd.Contactgegeventype} '{fixture.Scenario.ContactgegevenWerdToegevoegd.Waarde}' werd toegevoegd.",
                                               nameof(ContactgegevenWerdToegevoegd),
                                               fixture.Scenario.ContactgegevenWerdToegevoegd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
