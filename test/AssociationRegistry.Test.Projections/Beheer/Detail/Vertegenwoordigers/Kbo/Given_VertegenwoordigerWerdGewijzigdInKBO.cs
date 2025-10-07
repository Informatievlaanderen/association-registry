﻿namespace AssociationRegistry.Test.Projections.Beheer.Detail.Vertegenwoordigers.Kbo;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Test.Projections.Scenario.Vertegenwoordigers.Kbo;
using Contracts.JsonLdContext;

[Collection(nameof(ProjectionContext))]
public class Given_VertegenwoordigerWerdGewijzigdInKBO(
    BeheerDetailScenarioFixture<VertegenwoordigerWerdGewijzigdInKBOScenario> fixture)
    : BeheerDetailScenarioClassFixture<VertegenwoordigerWerdGewijzigdInKBOScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(3);

    [Fact]
    public void Vertegenwoordiger_Is_Toegevoegd()
    {
        var vertegenwoordiger = fixture.Result.Vertegenwoordigers.Single(x => x.VertegenwoordigerId == fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO.VertegenwoordigerId);

        vertegenwoordiger.Should().BeEquivalentTo(
            new Vertegenwoordiger
            {
                JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                    JsonLdType.Vertegenwoordiger, fixture.Scenario.VCode,
                    fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO.VertegenwoordigerId.ToString()),
                VertegenwoordigerId =
                    fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO.VertegenwoordigerId,
                Insz = fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO.Insz,
                Achternaam = fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO.Achternaam,
                Voornaam = fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO.Voornaam,
                Roepnaam = string.Empty,
                Rol = string.Empty,
                IsPrimair = false,
                Email = string.Empty,
                Telefoon = string.Empty,
                Mobiel = string.Empty,
                SocialMedia = string.Empty,
                Bron = fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO.Bron,
                VertegenwoordigerContactgegevens = new VertegenwoordigerContactgegevens
                {
                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                        JsonLdType.VertegenwoordigerContactgegeven, fixture.Scenario.VCode,
                        fixture.Scenario.VertegenwoordigerWerdGewijzigdInKBO.VertegenwoordigerId.ToString()),
                    IsPrimair = false,
                    Email = string.Empty,
                    Telefoon = string.Empty,
                    Mobiel = string.Empty,
                    SocialMedia = string.Empty,
                },
            });
    }
}
