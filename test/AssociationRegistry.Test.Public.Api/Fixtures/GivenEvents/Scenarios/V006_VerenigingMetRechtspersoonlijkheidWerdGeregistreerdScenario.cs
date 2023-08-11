namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime.Extensions;
using Parlot.Fluent;

public class V006_VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario : IScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new(
        "V0001006",
        "0000000000",
        "VZW",
        "VZW 0000000000",
        string.Empty,
        null);

    public VCode VCode
        => VCode.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(), Guid.NewGuid());
}
