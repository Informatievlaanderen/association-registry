namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using NodaTime;

public class V026_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithInStopzetting_ForSearchOnInStopzetting
    : IScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdInStopzettingGeplaatst VerenigingWerdInStopzettingGeplaatst;

    public V026_VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd_WithInStopzetting_ForSearchOnInStopzetting()
    {
        var fixture = new Fixture().CustomizeDomain();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = "V0001026",
        };

        VerenigingWerdInStopzettingGeplaatst = fixture.Create<VerenigingWerdInStopzettingGeplaatst>();
    }

    public VCode VCode => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode),
            VerenigingWerdInStopzettingGeplaatst,
        };
    }

    public CommandMetadata GetCommandMetadata() => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
