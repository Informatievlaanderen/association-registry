namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport.ScenarioClassFixtures;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;
using AutoFixture;
using Projections.PowerBiExport;

public class WerkingsgebiedenWerdenGewijzigdScenario : ProjectionScenarioFixture<PowerBiExportContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public WerkingsgebiedenWerdenGewijzigd WerkingsgebiedenWerdenGewijzigd { get; set; }

    public WerkingsgebiedenWerdenGewijzigdScenario(PowerBiExportContext context): base(context)
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        WerkingsgebiedenWerdenGewijzigd = AutoFixture.Create<WerkingsgebiedenWerdenGewijzigd>();
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(VerenigingWerdGeregistreerd.VCode,
                              VerenigingWerdGeregistreerd);
        await session.SaveChangesAsync();
        await using var session2 = await Context.DocumentSession();

        session2.Events.Append(VerenigingWerdGeregistreerd.VCode,
                               WerkingsgebiedenWerdenGewijzigd);

        await session2.SaveChangesAsync();

        await Context.WaitForNonStaleProjectionDataAsync();
    }
}
