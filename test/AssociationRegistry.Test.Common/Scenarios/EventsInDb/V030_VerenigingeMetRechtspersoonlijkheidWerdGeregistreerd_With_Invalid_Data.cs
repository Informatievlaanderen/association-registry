namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using EventStore;
using global::AutoFixture;
using MartenDb.Store;
using Vereniging;

public class V030_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_Invalid_Data : IEventsInDbScenario
{
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO EmailKonNietOvergenomenWordenUitKbo;
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO GsmKonNietOvergenomenWordenUitKbo;
    public readonly MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo;
    public readonly CommandMetadata Metadata;
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO TelefoonKonNietOvergenomenWordenUitKbo;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO WebsiteKonNietOvergenomenWordenUitKbo;

    public V030_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_Invalid_Data()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VCode = "V9999030";
        Naam = "Recht door zee";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            KorteNaam = "RDZ",
            KboNummer = "7981199830",
        };

        MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo = new MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(
            Straatnaam: "Stationsstraat",
            Huisnummer: "1",
            Busnummer: "B",
            Postcode: "1790",
            Gemeente: "Affligem",
            Land: "België");

        EmailKonNietOvergenomenWordenUitKbo =
            new ContactgegevenKonNietOvergenomenWordenUitKBO(Contactgegeventype.Email.Waarde, ContactgegeventypeVolgensKbo.Email.Waarde,
                                                             fixture.Create<string>());

        WebsiteKonNietOvergenomenWordenUitKbo =
            new ContactgegevenKonNietOvergenomenWordenUitKBO(Contactgegeventype.Website.Waarde, ContactgegeventypeVolgensKbo.Website.Waarde,
                                                             fixture.Create<string>());

        TelefoonKonNietOvergenomenWordenUitKbo = new ContactgegevenKonNietOvergenomenWordenUitKBO(
            Contactgegeventype.Telefoon.Waarde, ContactgegeventypeVolgensKbo.Telefoon.Waarde, fixture.Create<string>());

        GsmKonNietOvergenomenWordenUitKbo =
            new ContactgegevenKonNietOvergenomenWordenUitKBO(Contactgegeventype.Telefoon.Waarde, ContactgegeventypeVolgensKbo.GSM.Waarde,
                                                             fixture.Create<string>());

        KboNummer = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer;
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string KboNummer { get; set; }
    public string Naam { get; set; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo,
            EmailKonNietOvergenomenWordenUitKbo,
            WebsiteKonNietOvergenomenWordenUitKbo,
            TelefoonKonNietOvergenomenWordenUitKbo,
            GsmKonNietOvergenomenWordenUitKbo,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
