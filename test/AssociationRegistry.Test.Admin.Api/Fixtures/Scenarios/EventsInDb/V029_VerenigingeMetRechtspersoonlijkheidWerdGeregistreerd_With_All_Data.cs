namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class V029_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;
    public readonly ContactgegevenWerdOvergenomenUitKBO EmailWerdOvergenomenUitKBO;
    public readonly ContactgegevenWerdOvergenomenUitKBO WebsiteWerdOvergenomenUitKBO;
    public readonly ContactgegevenWerdOvergenomenUitKBO TelefoonWerdOvergenomenUitKBO;
    public readonly CommandMetadata Metadata;

    public V029_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VCode = "V9999029";
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
        EmailWerdOvergenomenUitKBO = new ContactgegevenWerdOvergenomenUitKBO(1, ContactgegevenType.Email.Waarde, "email@testdata.com", "", false);
        WebsiteWerdOvergenomenUitKBO = new ContactgegevenWerdOvergenomenUitKBO(1, ContactgegevenType.Website.Waarde, "https://www.testdata.com", "", false);
        TelefoonWerdOvergenomenUitKBO = new ContactgegevenWerdOvergenomenUitKBO(1, ContactgegevenType.Telefoon.Waarde, "0123456789", "", false);
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
            EmailWerdOvergenomenUitKBO,
            WebsiteWerdOvergenomenUitKBO,
            TelefoonWerdOvergenomenUitKBO
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
