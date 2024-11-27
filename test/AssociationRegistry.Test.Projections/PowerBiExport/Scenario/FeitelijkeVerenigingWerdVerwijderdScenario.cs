namespace AssociationRegistry.Test.Projections.PowerBiExport;

using AutoFixture;
using Events;
using Framework.Fixtures;

public class FeitelijkeVerenigingWerdVerwijderdScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd1 { get; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd2 { get; }
    public VerenigingWerdVerwijderd VerenigingWerdVerwijderd { get; }

    public FeitelijkeVerenigingWerdVerwijderdScenario(ProjectionContext context) : base(context)
    {
        FeitelijkeVerenigingWerdGeregistreerd1 = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        FeitelijkeVerenigingWerdGeregistreerd2 = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        VerenigingWerdVerwijderd =
            new VerenigingWerdVerwijderd(FeitelijkeVerenigingWerdGeregistreerd1.VCode, Reden: "Verwijderd voor testen.");
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
    }
}
