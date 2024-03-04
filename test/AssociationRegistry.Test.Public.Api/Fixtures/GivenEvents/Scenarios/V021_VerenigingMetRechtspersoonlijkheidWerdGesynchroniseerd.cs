namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime.Extensions;
using Vereniging;

public class V021_VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd : IScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new(
        VCode: "V0001021",
        KboNummer: "0987654420",
        Rechtsvorm: Verenigingstype.VZW.Code,
        Naam: "Feesten Affligem",
        string.Empty,
        Startdatum: null);

    public readonly RechtsvormWerdGewijzigdInKBO RechtsvormWerdGewijzigdInKBO = new(Verenigingstype.IVZW.Code);
    public readonly NaamWerdGewijzigdInKbo NaamWerdGewijzigdInKbo = new("Feesten Asse");
    public readonly KorteNaamWerdGewijzigdInKbo KorteNaamWerdGewijzigdInKbo = new("FA");

    public readonly ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKbo =
        new(1, Contactgegeventype.Email, ContactgegeventypeVolgensKbo.Email, "example.me@example.org");

    public readonly ContactgegevenWerdGewijzigdInKbo ContactgegevenWerdGewijzigdInKbo =
        new(1, Contactgegeventype.Email, ContactgegeventypeVolgensKbo.Email, "test.me@example.org");

    public VCode VCode
        => VCode.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            ContactgegevenWerdOvergenomenUitKbo,
            RechtsvormWerdGewijzigdInKBO,
            NaamWerdGewijzigdInKbo,
            KorteNaamWerdGewijzigdInKbo,
            ContactgegevenWerdGewijzigdInKbo,
            new SynchronisatieMetKboWasSuccesvol(),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001",
               new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(),
               Guid.NewGuid());
}
