namespace AssociationRegistry.Test.Projections.Beheer.Detail.Koepels;

using Admin.Schema.Detail;
using Scenario.Koepels;
using Koepelstatus = DecentraalBeheer.Vereniging.Koepels.Koepelstatus;

[Collection(nameof(ProjectionContext))]
public class Given_KoepelstatusWerdTerugGezetNaarNietBepaald(
    BeheerDetailScenarioFixture<KoepelstatusWerdTerugGezetNaarNietBepaaldScenario> fixture
) : BeheerDetailScenarioClassFixture<KoepelstatusWerdTerugGezetNaarNietBepaaldScenario>
{
    [Fact]
    public void Metadata_Is_Updated() => fixture.Result.Metadata.Version.Should().Be(3);

    [Fact]
    public void Document_Is_Updated() =>
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
