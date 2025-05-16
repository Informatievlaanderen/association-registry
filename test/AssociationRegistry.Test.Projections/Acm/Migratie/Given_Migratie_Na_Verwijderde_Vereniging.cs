namespace AssociationRegistry.Test.Projections.Acm.Migratie;

using AssociationRegistry.Acm.Schema.VerenigingenPerInsz;
using Marten;
using Marten.Linq.SoftDeletes;
using Scenario.Migratie;

[Collection(nameof(ProjectionContext))]
public class Given_Migratie_Na_Verwijderde_Vereniging(
    VerenigingenPerInszScenarioFixture<Migratie_Na_Verwijderde_Vereniging> fixture)
    : VerenigingenPerInszScenarioClassFixture<Migratie_Na_Verwijderde_Vereniging>
{
    [Fact]
    public async Task Then_VerenigingsType_Is_Vzer()
    {
        await using var session = fixture.Context.AcmStore.LightweightSession();
        var verenigingDocument = await session.Query<VerenigingDocument>()
                    .Where(document => document.MaybeDeleted())
                    .Where(v => v.VCode == fixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                    .SingleAsync();

        verenigingDocument.VerenigingsType.Should().BeEquivalentTo(new Verenigingstype(AssociationRegistry.Vereniging.Verenigingstype.VZER.Code, AssociationRegistry.Vereniging.Verenigingstype.VZER.Naam));

        fixture.Result.Verenigingen.Should().BeEmpty();
    }
}
