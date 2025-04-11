namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Contactgegevens.Kbo;

using Admin.Schema.Historiek;
using Events;
using Scenario.Contactgegevens.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenUitKBOWerdGewijzigd(
    BeheerHistoriekScenarioFixture<ContactgegevenUitKBOWerdGewijzigdScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<ContactgegevenUitKBOWerdGewijzigdScenario>
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
                                               Beschrijving: $"{fixture.Scenario._contactgegevenWerdOvergenomenUitKboScenario.ContactgegevenWerdOvergenomenUitKbo.TypeVolgensKbo} '{fixture.Scenario._contactgegevenWerdOvergenomenUitKboScenario.ContactgegevenWerdOvergenomenUitKbo.Waarde}' werd gewijzigd.",
                                               nameof(ContactgegevenUitKBOWerdGewijzigd),
                                               fixture.Scenario.ContactgegevenUitKBOWerdGewijzigd,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
