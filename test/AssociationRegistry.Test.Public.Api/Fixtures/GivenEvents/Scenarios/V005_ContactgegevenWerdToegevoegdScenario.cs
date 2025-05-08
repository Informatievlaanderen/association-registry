namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using EventFactories;
using Events;
using NodaTime.Extensions;
using Vereniging;

public class V005_ContactgegevenWerdToegevoegdScenario : IScenario
{
    public readonly ContactgegevenWerdToegevoegd ContactgegevenWerdToegevoegd =
        new(ContactgegevenId: 1, Contactgegeventype.Email, Waarde: "test@example.org", Beschrijving: "de email om naar te sturen",
            IsPrimair: false);

    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd = new(
        VCode: "V0001005",
        Naam: "Feesten Hulste",
        string.Empty,
        string.Empty,
        Startdatum: null,
        EventFactory.Doelgroep(Doelgroep.Null),
        IsUitgeschrevenUitPubliekeDatastroom: false,
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
            new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(VCode),
            ContactgegevenWerdToegevoegd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001",
               new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(),
               Guid.NewGuid());
}
