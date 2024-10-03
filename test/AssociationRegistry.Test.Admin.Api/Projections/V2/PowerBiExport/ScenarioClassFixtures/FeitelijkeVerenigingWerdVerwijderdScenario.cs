namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport.ScenarioClassFixtures;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;
using AutoFixture;
using Projections.PowerBiExport;

public class FeitelijkeVerenigingWerdVerwijderdScenario : ProjectionScenarioFixture<PowerBiExportContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd1 { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd2 { get; }
    public VerenigingWerdVerwijderd VerenigingWerdVerwijderd { get; }

    public FeitelijkeVerenigingWerdVerwijderdScenario(PowerBiExportContext context) : base(context)
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
