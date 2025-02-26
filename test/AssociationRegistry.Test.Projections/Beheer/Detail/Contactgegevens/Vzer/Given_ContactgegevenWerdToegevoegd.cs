namespace AssociationRegistry.Test.Projections.Beheer.Detail.Contactgegevens.Vzer;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using JsonLdContext;
using Scenario.Contactgegevens.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdToegevoegd(
    BeheerDetailScenarioFixture<ContactgegevenWerdToegevoegdScenario> fixture)
    : BeheerDetailScenarioClassFixture<ContactgegevenWerdToegevoegdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Waarde_Is_Updated()
    {
        var contactGegeven = fixture.Result.Contactgegevens.Single(x => x.ContactgegevenId == fixture.Scenario.ContactgegevenWerdToegevoegd.ContactgegevenId);

        contactGegeven.Should().BeEquivalentTo(new Contactgegeven
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                JsonLdType.Contactgegeven, fixture.Scenario.VCode,
                contactGegeven.ContactgegevenId.ToString()),
            ContactgegevenId = contactGegeven.ContactgegevenId,
            Contactgegeventype = contactGegeven.Contactgegeventype,
            Waarde = contactGegeven.Waarde,
            Beschrijving = contactGegeven.Beschrijving,
            Bron = contactGegeven.Bron,
            IsPrimair = contactGegeven.IsPrimair,
        });
    }
}
