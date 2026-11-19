namespace AssociationRegistry.Test.Projections.Beheer.Detail.Koepels;

using Admin.Schema.Detail;
using Scenario.Registratie;
using Koepelstatus = DecentraalBeheer.Vereniging.Koepels.Koepelstatus;

[Collection(nameof(ProjectionContext))]
public class Given_A_Vereniging_Without_Koepel_Events(
    BeheerDetailScenarioFixture<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario> fixture
) : BeheerDetailScenarioClassFixture<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario>
{
    [Fact]
    public void Koepelgegevens_Are_NietBepaald_And_Not_Accepted() =>
        fixture
            .Result.Koepelgegevens.Should()
            .BeEquivalentTo(
                new Koepelgegevens
                {
                    Koepelstatus = Koepelstatus.NietBepaald.Value,
                    PrivacyVerklaringGeaccepteerd = false,
                }
            );
}
