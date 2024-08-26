namespace AssociationRegistry.Test.Admin.Api.Projections.PowerBiExport.ScenarioClassFixtures;

using AutoFixture;
using Events;

public class MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario : ProjectionScenarioFixture
{
    public FeitelijkeVerenigingWerdGeregistreerd[] VerenigingenwerdenGeregistreerd { get; }

    public MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario(ProjectionContext context): base(context)
    {
        VerenigingenwerdenGeregistreerd = AutoFixture.CreateMany<FeitelijkeVerenigingWerdGeregistreerd>()
                                             .ToArray();
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        foreach (var feitelijkeVerenigingWerdGeregistreerd in VerenigingenwerdenGeregistreerd)
        {
            session.Events.Append(feitelijkeVerenigingWerdGeregistreerd.VCode, feitelijkeVerenigingWerdGeregistreerd);
        }

        await session.SaveChangesAsync();

        await Context.WaitForNonStaleProjectionDataAsync();
    }
}
