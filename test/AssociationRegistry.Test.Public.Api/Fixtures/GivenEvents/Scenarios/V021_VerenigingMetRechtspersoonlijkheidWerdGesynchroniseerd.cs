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
        Verenigingstype.VZW.Code,
        Naam: "Feesten Affligem",
        string.Empty,
        Startdatum: null);

    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo =
        new(new Registratiedata.Locatie(LocatieId: 1, Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                                        IsPrimair: false, Naam: "",
                                        new Registratiedata.Adres(
                                            Straatnaam: "maatstraat", Huisnummer: "1", Busnummer: null, Postcode: "1000",
                                            Gemeente: "Brussel",
                                            Land: "België"), AdresId: null));

    public readonly MaatschappelijkeZetelWerdGewijzigdInKbo MaatschappelijkeZetelWerdGewijzigdInKbo =
        new(new Registratiedata.Locatie(LocatieId: 1, Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                                        IsPrimair: false, Naam: "",
                                        new Registratiedata.Adres(
                                            Straatnaam: "beterlaan", Huisnummer: "42", Busnummer: "b", Postcode: "2000",
                                            Gemeente: "Antwerpen",
                                            Land: "België"), AdresId: null));

    public readonly RechtsvormWerdGewijzigdInKBO RechtsvormWerdGewijzigdInKBO = new(Verenigingstype.IVZW.Code);
    public readonly NaamWerdGewijzigdInKbo NaamWerdGewijzigdInKbo = new("Feesten Asse");
    public readonly KorteNaamWerdGewijzigdInKbo KorteNaamWerdGewijzigdInKbo = new("FA");

    public readonly ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKbo =
        new(ContactgegevenId: 1, Contactgegeventype.Email, ContactgegeventypeVolgensKbo.Email, Waarde: "example.me@example.org");

    public readonly ContactgegevenWerdGewijzigdInKbo ContactgegevenWerdGewijzigdInKbo =
        new(ContactgegevenId: 1, Contactgegeventype.Email, ContactgegeventypeVolgensKbo.Email, Waarde: "test.me@example.org");

    public VCode VCode
        => VCode.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
            ContactgegevenWerdOvergenomenUitKbo,
            RechtsvormWerdGewijzigdInKBO,
            NaamWerdGewijzigdInKbo,
            KorteNaamWerdGewijzigdInKbo,
            MaatschappelijkeZetelWerdGewijzigdInKbo,
            ContactgegevenWerdGewijzigdInKbo,
            new SynchronisatieMetKboWasSuccesvol(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001",
               new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(),
               Guid.NewGuid());
}
