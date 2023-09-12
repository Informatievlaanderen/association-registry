namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class V029_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;
    public readonly ContactgegevenWerdOvergenomenUitKBO EmailWerdOvergenomenUitKBO;
    public readonly ContactgegevenWerdOvergenomenUitKBO WebsiteWerdOvergenomenUitKBO;
    public readonly ContactgegevenWerdOvergenomenUitKBO TelefoonWerdOvergenomenUitKBO;
    public readonly ContactgegevenWerdOvergenomenUitKBO GSMWerdOvergenomenUitKBO;
    public readonly VertegenwoordigerWerdOvergenomenUitKBO VertegenwoordigerWerdOvergenomenUitKBO;
    public readonly ContactgegevenUitKBOWerdGewijzigd EmailWerdGewijzigd;

    public readonly CommandMetadata Metadata;

    public V029_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data()
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
            Rechtsvorm = Verenigingstype.VZW.Code,
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

        EmailWerdOvergenomenUitKBO =
            new ContactgegevenWerdOvergenomenUitKBO(1, ContactgegevenType.Email.Waarde, ContactgegevenTypeVolgensKbo.Email,
                                                    "email@testdata.com");

        WebsiteWerdOvergenomenUitKBO =
            new ContactgegevenWerdOvergenomenUitKBO(2, ContactgegevenType.Website.Waarde, ContactgegevenTypeVolgensKbo.Website,
                                                    "https://www.testdata.com");

        TelefoonWerdOvergenomenUitKBO =
            new ContactgegevenWerdOvergenomenUitKBO(3, ContactgegevenType.Telefoon.Waarde, ContactgegevenTypeVolgensKbo.Telefoon,
                                                    "0123456789");

        GSMWerdOvergenomenUitKBO = new(4, ContactgegevenType.Telefoon.Waarde, ContactgegevenTypeVolgensKbo.GSM, "0987654321");

        VertegenwoordigerWerdOvergenomenUitKBO = new VertegenwoordigerWerdOvergenomenUitKBO(1, "0123456789", "Jhon", "Doo");

        EmailWerdGewijzigd = new ContactgegevenUitKBOWerdGewijzigd(1, "TestEmail", true);

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
            TelefoonWerdOvergenomenUitKBO,
            GSMWerdOvergenomenUitKBO,
            VertegenwoordigerWerdOvergenomenUitKBO,
            EmailWerdGewijzigd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
