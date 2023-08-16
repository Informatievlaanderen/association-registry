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
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO EmailKonNietOvergenomenWordenUitKbo;
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO WebsiteKonNietOvergenomenWordenUitKbo;
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO TelefoonKonNietOvergenomenWordenUitKbo;
    public readonly CommandMetadata Metadata;

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
        MaatschappelijkeZetelWerdOvergenomenUitKbo = new MaatschappelijkeZetelWerdOvergenomenUitKbo(
            Locatie: fixture.Create<Registratiedata.Locatie>() with
            {
                Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo,
                Naam = string.Empty,
                IsPrimair = false,
                Adres = new Registratiedata.Adres(
                    "Stationsstraat",
                    "1",
                    "B",
                    "1790",
                    "Affligem",
                    "België"),
                AdresId = null,
            });
        EmailKonNietOvergenomenWordenUitKbo = new ContactgegevenKonNietOvergenomenWordenUitKBO(ContactgegevenType.Email.Waarde, fixture.Create<string>());
        WebsiteKonNietOvergenomenWordenUitKbo = new ContactgegevenKonNietOvergenomenWordenUitKBO(ContactgegevenType.Website.Waarde, fixture.Create<string>());
        TelefoonKonNietOvergenomenWordenUitKbo = new ContactgegevenKonNietOvergenomenWordenUitKBO(ContactgegevenType.Telefoon.Waarde, fixture.Create<string>());
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
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
            EmailKonNietOvergenomenWordenUitKbo,
            WebsiteKonNietOvergenomenWordenUitKbo,
            TelefoonKonNietOvergenomenWordenUitKbo
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
