namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;
using Vereniging;
using Vereniging.Verenigingstype;

public class V029_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data : IEventsInDbScenario
{
    public readonly ContactgegevenUitKBOWerdGewijzigd EmailWerdGewijzigd;
    public readonly ContactgegevenWerdOvergenomenUitKBO EmailWerdOvergenomenUitKBO;
    public readonly ContactgegevenWerdOvergenomenUitKBO GSMWerdOvergenomenUitKBO;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;
    public readonly CommandMetadata Metadata;
    public readonly ContactgegevenWerdOvergenomenUitKBO TelefoonWerdOvergenomenUitKBO;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VertegenwoordigerWerdOvergenomenUitKBO VertegenwoordigerWerdOvergenomenUitKBO;
    public readonly ContactgegevenWerdOvergenomenUitKBO WebsiteWerdOvergenomenUitKBO;

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
            KboNummer = "7981199829",
            Rechtsvorm = Verenigingstype.VZW.Code,
        };

        MaatschappelijkeZetelWerdOvergenomenUitKbo = new MaatschappelijkeZetelWerdOvergenomenUitKbo(
            Locatie: fixture.Create<Registratiedata.Locatie>() with
            {
                Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo,
                Naam = string.Empty,
                IsPrimair = false,
                Adres = new Registratiedata.Adres(
                    Straatnaam: "Stationsstraat",
                    Huisnummer: "1",
                    Busnummer: "B",
                    Postcode: "1790",
                    Gemeente: "Affligem",
                    Land: "België"),
                AdresId = null,
            });

        EmailWerdOvergenomenUitKBO =
            new ContactgegevenWerdOvergenomenUitKBO(ContactgegevenId: 1, Contactgegeventype.Email.Waarde,
                                                    ContactgegeventypeVolgensKbo.Email,
                                                    Waarde: "email@testdata.com");

        WebsiteWerdOvergenomenUitKBO =
            new ContactgegevenWerdOvergenomenUitKBO(ContactgegevenId: 2, Contactgegeventype.Website.Waarde,
                                                    ContactgegeventypeVolgensKbo.Website,
                                                    Waarde: "https://www.testdata.com");

        TelefoonWerdOvergenomenUitKBO =
            new ContactgegevenWerdOvergenomenUitKBO(ContactgegevenId: 3, Contactgegeventype.Telefoon.Waarde,
                                                    ContactgegeventypeVolgensKbo.Telefoon,
                                                    Waarde: "0123456789");

        GSMWerdOvergenomenUitKBO = new ContactgegevenWerdOvergenomenUitKBO(ContactgegevenId: 4, Contactgegeventype.Telefoon.Waarde,
                                                                           ContactgegeventypeVolgensKbo.GSM, Waarde: "0987654321");

        VertegenwoordigerWerdOvergenomenUitKBO =
            new VertegenwoordigerWerdOvergenomenUitKBO(VertegenwoordigerId: 1, Insz: "0123456789", Voornaam: "Jhon", Achternaam: "Doo");

        EmailWerdGewijzigd = new ContactgegevenUitKBOWerdGewijzigd(ContactgegevenId: 1, Beschrijving: "TestEmail", IsPrimair: true);

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
            new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(KboNummer),
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
