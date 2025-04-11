namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Contactgegevens.Kbo;

using Admin.Schema.Historiek;
using Events;
using Scenario.Contactgegevens.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdInBeheerGenomenDoorKbo(
    BeheerHistoriekScenarioFixture<ContactgegevenWerdInBeheerGenomenDoorKboScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<ContactgegevenWerdInBeheerGenomenDoorKboScenario>
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
                                               Beschrijving: $"{@fixture.Scenario.ContactgegevenWerdInBeheerGenomenDoorKbo.Contactgegeventype} '{fixture.Scenario.ContactgegevenWerdInBeheerGenomenDoorKbo.Waarde}' werd in beheer genomen door KBO.",
                                               nameof(ContactgegevenWerdInBeheerGenomenDoorKbo),
                                               fixture.Scenario.ContactgegevenWerdInBeheerGenomenDoorKbo,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
