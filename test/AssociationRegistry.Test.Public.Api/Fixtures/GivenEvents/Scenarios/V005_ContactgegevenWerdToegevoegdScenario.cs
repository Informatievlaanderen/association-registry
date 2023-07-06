namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using Events;
using AssociationRegistry.Framework;
using Vereniging;
using NodaTime.Extensions;

public class V005_ContactgegevenWerdToegevoegdScenario : IScenario
{
    public readonly ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd = new(ContactgegevenId: 1, ContactgegevenType.Email, "test@example.org", "de email om naar te sturen", IsPrimair: false);

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        "V0001005",
        "Feesten Hulste",
        string.Empty,
        string.Empty,
        Startdatum: null,
        Registratiedata.Doelgroep.With(Doelgroep.Null),
        false,
        Array.Empty<Registratiedata.Contactgegeven>(),
        Array.Empty<Registratiedata.Locatie>(),
        Array.Empty<Registratiedata.Vertegenwoordiger>(),
        Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());

    public VCode VCode
        => VCode.Create(FeitelijkeVerenigingWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            ContactgegevenWerdToegevoegd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant());
}
