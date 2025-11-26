namespace AssociationRegistry.Test.Projections.Beheer.Detail.Vertegenwoordigers.Vzer;

using Admin.ProjectionHost.Projections.Detail;
using Admin.Schema.Detail;
using Contracts.JsonLdContext;
using Scenario.Vertegenwoordigers.Vzer;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdToegevoegd(
    BeheerDetailScenarioFixture<VertegenwoordigerWerdToegevoegdScenario> fixture)
    : BeheerDetailScenarioClassFixture<VertegenwoordigerWerdToegevoegdScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Vertegenwoordiger_Is_Toegevoegd()
    {
        var vertegenwoordiger = fixture.Result.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == fixture.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId);

        vertegenwoordiger.Should().BeEquivalentTo(new Vertegenwoordiger
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                JsonLdType.Vertegenwoordiger, fixture.Scenario.AggregateId,
                fixture.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId.ToString()),
            VertegenwoordigerId = fixture.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            Insz = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Insz,
            Achternaam = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Achternaam,
            Voornaam = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
            Roepnaam = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Roepnaam,
            Rol = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Rol,
            IsPrimair = fixture.Scenario.VertegenwoordigerWerdToegevoegd.IsPrimair,
            Email = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Email,
            Telefoon = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Telefoon,
            Mobiel = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Mobiel,
            SocialMedia = fixture.Scenario.VertegenwoordigerWerdToegevoegd.SocialMedia,
            Bron = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Bron,

            VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                    JsonLdType.VertegenwoordigerContactgegeven, fixture.Scenario.AggregateId,
                    fixture.Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId.ToString()),
                IsPrimair = fixture.Scenario.VertegenwoordigerWerdToegevoegd.IsPrimair,
                Email = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Email,
                Telefoon = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Telefoon,
                Mobiel = fixture.Scenario.VertegenwoordigerWerdToegevoegd.Mobiel,
                SocialMedia = fixture.Scenario.VertegenwoordigerWerdToegevoegd.SocialMedia,
            },
        });
    }
}
