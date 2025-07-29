namespace AssociationRegistry.Test.Projections.Beheer.Detail.Vertegenwoordigers.Kbo;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using JsonLdContext;
using AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Kbo;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdOvergenomenUitKBO(
    BeheerDetailScenarioFixture<VertegenwoordigerWerdOvergenomenUitKBOScenario> fixture)
    : BeheerDetailScenarioClassFixture<VertegenwoordigerWerdOvergenomenUitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Vertegenwoordiger_Is_Toegevoegd()
    {
        var vertegenwoordiger = fixture.Result.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.VertegenwoordigerId);

        vertegenwoordiger.Should().BeEquivalentTo(
            new Vertegenwoordiger
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                    JsonLdType.Vertegenwoordiger, fixture.Scenario.VCode,
                    fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.VertegenwoordigerId.ToString()),
                VertegenwoordigerId =
                    fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.VertegenwoordigerId,
                Insz = fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.Insz,
                Achternaam = fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.Achternaam,
                Voornaam = fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.Voornaam,
                Roepnaam = string.Empty,
                Rol = string.Empty,
                IsPrimair = false,
                Email = string.Empty,
                Telefoon = string.Empty,
                Mobiel = string.Empty,
                SocialMedia = string.Empty,
                Bron = fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.Bron,
                VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
                {
                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                        JsonLdType.VertegenwoordigerContactgegeven, fixture.Scenario.VCode,
                        fixture.Scenario.VertegenwoordigerWerdOvergenomenUitKBO.VertegenwoordigerId.ToString()),
                    IsPrimair = false,
                    Email = string.Empty,
                    Telefoon = string.Empty,
                    Mobiel = string.Empty,
                    SocialMedia = string.Empty,
                },
            });
    }
}
