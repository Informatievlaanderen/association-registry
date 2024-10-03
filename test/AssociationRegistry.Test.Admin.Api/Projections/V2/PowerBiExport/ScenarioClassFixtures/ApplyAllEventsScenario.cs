namespace AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport.ScenarioClassFixtures;

using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Projections.V2.PowerBiExport;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Projections.PowerBiExport;

public class ApplyAllEventsScenario : ProjectionScenarioFixture<PowerBiExportContext>
{
    private readonly Fixture _fixture;



    public VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd { get; private set; }
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; private set; }

    public IReadOnlyCollection<IEvent> GetEvents(string vCode)
    {
        return new IEvent[]
        {
            _fixture.Create<NaamWerdGewijzigd>() with { VCode = vCode},
            _fixture.Create<LocatieWerdToegevoegd>(),
            _fixture.Create<LocatieWerdToegevoegd>(),
            _fixture.Create<VerenigingWerdGestopt>()
        };
    }

    public ApplyAllEventsScenario(PowerBiExportContext context) : base(context)
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

        session.Events.Append(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode, VerenigingMetRechtspersoonlijkheidWerdGeregistreerd);
        session.Events.Append(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode, GetEvents(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode));

        await session.SaveChangesAsync();

        await Context.WaitForNonStaleProjectionDataAsync();
    }
}
