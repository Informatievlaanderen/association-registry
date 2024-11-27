namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Framework.Fixtures;
using AutoFixture;

public class HoofdactiviteitenWerdenGewijzigdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public HoofdactiviteitenVerenigingsloketWerdenGewijzigd HoofdactiviteitenVerenigingsloketWerdenGewijzigd { get; set; }

    public HoofdactiviteitenWerdenGewijzigdScenario(ProjectionContext context) : base(context)
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
    }
}
