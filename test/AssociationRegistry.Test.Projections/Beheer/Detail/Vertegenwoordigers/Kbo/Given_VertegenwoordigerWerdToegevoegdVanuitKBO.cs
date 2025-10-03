namespace AssociationRegistry.Test.Projections.Beheer.Detail.Vertegenwoordigers.Kbo;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Kbo;
using Contracts.JsonLdContext;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdToegevoegdVanuitKBO(
    BeheerDetailScenarioFixture<VertegenwoordigerWerdToegevoegdVanuitKBOScenario> fixture)
    : BeheerDetailScenarioClassFixture<VertegenwoordigerWerdToegevoegdVanuitKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Vertegenwoordiger_Is_Toegevoegd()
    {
        var vertegenwoordiger = fixture.Result.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId);

        vertegenwoordiger.Should().BeEquivalentTo(
            new Vertegenwoordiger
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                    JsonLdType.Vertegenwoordiger, fixture.Scenario.VCode,
                    fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId.ToString()),
                VertegenwoordigerId =
                    fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId,
                Insz = fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO.Insz,
                Achternaam = fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO.Achternaam,
                Voornaam = fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO.Voornaam,
                Roepnaam = string.Empty,
                Rol = string.Empty,
                IsPrimair = false,
                Email = string.Empty,
                Telefoon = string.Empty,
                Mobiel = string.Empty,
                SocialMedia = string.Empty,
                Bron = fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO.Bron,
                VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
                {
                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                        JsonLdType.VertegenwoordigerContactgegeven, fixture.Scenario.VCode,
                        fixture.Scenario.VertegenwoordigerWerdToegevoegdVanuitKBO.VertegenwoordigerId.ToString()),
                    IsPrimair = false,
                    Email = string.Empty,
                    Telefoon = string.Empty,
                    Mobiel = string.Empty,
                    SocialMedia = string.Empty,
                },
            });
    }
}
