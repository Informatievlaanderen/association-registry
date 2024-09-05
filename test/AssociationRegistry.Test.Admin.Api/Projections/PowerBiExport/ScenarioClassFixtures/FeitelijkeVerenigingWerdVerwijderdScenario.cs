namespace AssociationRegistry.Test.Admin.Api.Projections.PowerBiExport.ScenarioClassFixtures;

using AutoFixture;
using Events;

public class FeitelijkeVerenigingWerdVerwijderdScenario : ProjectionScenarioFixture
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd1 { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd2 { get; }
    public VerenigingWerdVerwijderd VerenigingWerdVerwijderd { get; }

    public FeitelijkeVerenigingWerdVerwijderdScenario(ProjectionContext context) : base(context)
    {
        FeitelijkeVerenigingWerdGeregistreerd1 = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        FeitelijkeVerenigingWerdGeregistreerd2 = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        VerenigingWerdVerwijderd = new VerenigingWerdVerwijderd(FeitelijkeVerenigingWerdGeregistreerd1.VCode, "Verwijderd voor testen.");
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(FeitelijkeVerenigingWerdGeregistreerd1.VCode,
                              FeitelijkeVerenigingWerdGeregistreerd1);

        session.Events.Append(FeitelijkeVerenigingWerdGeregistreerd2.VCode,
                              FeitelijkeVerenigingWerdGeregistreerd2);

        session.Events.Append(FeitelijkeVerenigingWerdGeregistreerd1.VCode,
                              VerenigingWerdVerwijderd);

        await session.SaveChangesAsync();
        await Context.WaitForNonStaleProjectionDataAsync();
    }
}
