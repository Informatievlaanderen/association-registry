namespace AssociationRegistry.Test.Projections.PowerBiExport.ScenarioClassFixtures;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Events;
using Framework.Fixtures;

public class ApplyAllEventsScenario : ProjectionScenarioFixture<ProjectionContext>
{
    private readonly Fixture _fixture;
    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; private set; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; private set; }

    public IReadOnlyCollection<IEvent> GetEvents(string vCode)
    {
        return new IEvent[]
        {
            _fixture.Create<NaamWerdGewijzigd>() with { VCode = vCode },
            _fixture.Create<LocatieWerdToegevoegd>(),
            _fixture.Create<LocatieWerdToegevoegd>(),
            _fixture.Create<VerenigingWerdGestopt>(),
        };
    }

    public ApplyAllEventsScenario(ProjectionContext context) : base(context)
    {
        _fixture = new Fixture().CustomizeDomain();
        FeitelijkeVerenigingWerdGeregistreerd = _fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = _fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>();
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(FeitelijkeVerenigingWerdGeregistreerd.VCode, FeitelijkeVerenigingWerdGeregistreerd);
        session.Events.Append(FeitelijkeVerenigingWerdGeregistreerd.VCode, GetEvents(FeitelijkeVerenigingWerdGeregistreerd.VCode));

        session.Events.Append(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                              VerenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        session.Events.Append(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                              GetEvents(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode));

        await session.SaveChangesAsync();
    }
}
