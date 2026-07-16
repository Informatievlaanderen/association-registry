namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Erkenningen;
using Events;
using NodaTime;

public class V025_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningForSearchOnErkenningScenario
    : IScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly ErkenningWerdGeregistreerd ActieveErkenningWerdGeregistreerd;
    public readonly VerenigingWerdErkend VerenigingWerdErkend;

    public V025_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithActieveErkenningForSearchOnErkenningScenario()
    {
        var fixture = new Fixture().CustomizeDomain();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = "V0001025",
        };

        ActieveErkenningWerdGeregistreerd = fixture.Create<ErkenningWerdGeregistreerd>() with
        {
            Status = ErkenningStatus.Actief.Value,
        };

        VerenigingWerdErkend = fixture.Create<VerenigingWerdErkend>();
    }

    public VCode VCode => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            ActieveErkenningWerdGeregistreerd,
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode),
            VerenigingWerdErkend,
        };
    }

    public CommandMetadata GetCommandMetadata() => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
