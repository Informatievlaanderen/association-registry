namespace AssociationRegistry.Test.Projections.Beheer.Historiek.Contactgegevens.Kbo;

using Admin.Schema.Historiek;
using Events;
using Scenario.Contactgegevens.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdOvergenomenUitKbo(
    BeheerHistoriekScenarioFixture<ContactgegevenWerdOvergenomenUitKBOScenario> fixture)
    : BeheerHistoriekScenarioClassFixture<ContactgegevenWerdOvergenomenUitKBOScenario>
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
                                               Beschrijving:
                                               $"Contactgegeven ‘{fixture.Scenario.ContactgegevenWerdOvergenomenUitKbo.TypeVolgensKbo}' werd overgenomen uit KBO.",
                                               nameof(ContactgegevenWerdOvergenomenUitKBO),
                                               fixture.Scenario.ContactgegevenWerdOvergenomenUitKbo,
                                               fixture.MetadataInitiator,
                                               fixture.MetadataTijdstip));
}
