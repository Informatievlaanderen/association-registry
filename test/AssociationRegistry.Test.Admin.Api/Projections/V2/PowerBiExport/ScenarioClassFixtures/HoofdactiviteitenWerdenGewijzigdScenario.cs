namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport.ScenarioClassFixtures;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;
using AutoFixture;
using Projections.PowerBiExport;

public class HoofdactiviteitenWerdenGewijzigdScenario : ProjectionScenarioFixture<PowerBiExportContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public HoofdactiviteitenVerenigingsloketWerdenGewijzigd HoofdactiviteitenVerenigingsloketWerdenGewijzigd { get; set; }

    public HoofdactiviteitenWerdenGewijzigdScenario(PowerBiExportContext context): base(context)
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        HoofdactiviteitenVerenigingsloketWerdenGewijzigd = AutoFixture.Create<HoofdactiviteitenVerenigingsloketWerdenGewijzigd>();
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(VerenigingWerdGeregistreerd.VCode,
                              VerenigingWerdGeregistreerd);
        await session.SaveChangesAsync();
        await using var session2 = await Context.DocumentSession();

        session2.Events.Append(VerenigingWerdGeregistreerd.VCode,
                               HoofdactiviteitenVerenigingsloketWerdenGewijzigd);

        await session2.SaveChangesAsync();

        await Context.WaitForNonStaleProjectionDataAsync();
    }
}
