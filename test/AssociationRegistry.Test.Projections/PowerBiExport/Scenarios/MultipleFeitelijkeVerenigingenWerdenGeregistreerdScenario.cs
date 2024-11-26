namespace AssociationRegistry.Test.Projections.PowerBiExport.Scenarios;

using AutoFixture;
using Events;
using Framework;

public class MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd[] VerenigingenwerdenGeregistreerd { get; }

    public MultipleFeitelijkeVerenigingenWerdenGeregistreerdScenario(ProjectionContext context) : base(context)
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
    }
}
