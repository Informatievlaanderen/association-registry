namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class V030_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_Invalid_Data : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo;
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO EmailKonNietOvergenomenWordenUitKbo;
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO WebsiteKonNietOvergenomenWordenUitKbo;
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO TelefoonKonNietOvergenomenWordenUitKbo;
    public readonly CommandMetadata Metadata;
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO GsmKonNietOvergenomenWordenUitKbo;

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
            KboNummer = "7981199887",
        };
        MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo = new MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo(
            "Stationsstraat",
            "1",
            "B",
            "1790",
            "Affligem",
            "België");
        EmailKonNietOvergenomenWordenUitKbo = new ContactgegevenKonNietOvergenomenWordenUitKBO(ContactgegevenType.Email.Waarde, ContactgegevenTypeVolgensKbo.Email.Waarde, fixture.Create<string>());
        WebsiteKonNietOvergenomenWordenUitKbo = new ContactgegevenKonNietOvergenomenWordenUitKBO(ContactgegevenType.Website.Waarde, ContactgegevenTypeVolgensKbo.Website.Waarde, fixture.Create<string>());
        TelefoonKonNietOvergenomenWordenUitKbo = new ContactgegevenKonNietOvergenomenWordenUitKBO(ContactgegevenType.Telefoon.Waarde, ContactgegevenTypeVolgensKbo.Telefoon.Waarde, fixture.Create<string>());
        GsmKonNietOvergenomenWordenUitKbo = new ContactgegevenKonNietOvergenomenWordenUitKBO(ContactgegevenType.Telefoon.Waarde, ContactgegevenTypeVolgensKbo.GSM.Waarde, fixture.Create<string>());
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
