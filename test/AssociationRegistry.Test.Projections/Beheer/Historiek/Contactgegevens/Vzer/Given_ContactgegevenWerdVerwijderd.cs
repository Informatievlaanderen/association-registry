namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Contactgegevens.Vzer;

using Admin.Schema.Historiek;
using Events;
using Scenario.Contactgegevens.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdVerwijderd(
    BeheerHistoriekScenarioFixture<ContactgegevenWerdVerwijderdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<ContactgegevenWerdVerwijderdScenario>
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
                                               Beschrijving: $"{fixture.Scenario.ContactgegevenWerdVerwijderd.Type} '{fixture.Scenario.ContactgegevenWerdVerwijderd.Waarde}' werd verwijderd.",
                                               nameof(ContactgegevenWerdVerwijderd),
                                               fixture.Scenario.ContactgegevenWerdVerwijderd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
