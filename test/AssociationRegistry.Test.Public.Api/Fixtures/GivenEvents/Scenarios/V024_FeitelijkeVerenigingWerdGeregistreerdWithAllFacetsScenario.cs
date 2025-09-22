namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Factories;
using NodaTime;
using Vereniging;

public class V024_FeitelijkeVerenigingWerdGeregistreerdWithAllFacetsScenario : IScenario
{
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001", new Instant(), Guid.NewGuid());
}
