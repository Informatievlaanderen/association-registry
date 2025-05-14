namespace AssociationRegistry.Test.Projections.Beheer.Detail.Contactgegevens.Kbo;

using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Vereniging.Bronnen;
using Scenario.Contactgegevens.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_ContactgegevenWerdOvergenomenUitKbo(
    BeheerDetailScenarioFixture<ContactgegevenWerdOvergenomenUitKBOScenario> fixture)
    : BeheerDetailScenarioClassFixture<ContactgegevenWerdOvergenomenUitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void ContactGegeven_Is_Overgenomen()
    {
        var contactgegevenWerdOvergenomenUitKbo = fixture.Scenario.ContactgegevenWerdOvergenomenUitKbo;
        fixture.Result
               .Contactgegevens.Should().BeEquivalentTo([
                    new Contactgegeven
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.Contactgegeven.CreateWithIdValues(
                                fixture.Scenario.VCode,
                                contactgegevenWerdOvergenomenUitKbo.ContactgegevenId.ToString()),
                            Type = JsonLdType.Contactgegeven.Type,
                        },
                        ContactgegevenId = contactgegevenWerdOvergenomenUitKbo.ContactgegevenId,
                        Beschrijving = string.Empty,
                        Contactgegeventype = contactgegevenWerdOvergenomenUitKbo.Contactgegeventype,
                        Waarde = contactgegevenWerdOvergenomenUitKbo.Waarde,
                        Bron = Bron.KBO,
                    }
                ]);
    }
}
