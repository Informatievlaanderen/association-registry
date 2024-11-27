namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Projections.Framework.Fixtures;
using AutoFixture;

public class WerkingsgebiedenWerdenGewijzigdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public WerkingsgebiedenWerdenGewijzigd WerkingsgebiedenWerdenGewijzigd { get; set; }

    public WerkingsgebiedenWerdenGewijzigdScenario(ProjectionContext context) : base(context)
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
    }
}
