namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime.Extensions;
using System;
using Vereniging;

public class V014_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data_Scenario : IScenario
{
    public readonly ContactgegevenUitKBOWerdGewijzigd EmailWerdGewijzigd =
        new(ContactgegevenId: 1, Beschrijving: "TestEmail", IsPrimair: true);

    public readonly ContactgegevenWerdOvergenomenUitKBO EmailWerdOvergenomenUitKBO =
        new(ContactgegevenId: 1, Contactgegeventype.Email.Waarde, ContactgegeventypeVolgensKbo.Email, Waarde: "email@testdata.com");

    public readonly ContactgegevenWerdOvergenomenUitKBO GSMWerdOvergenomenUitKBO =
        new(ContactgegevenId: 4, Contactgegeventype.Telefoon.Waarde, ContactgegeventypeVolgensKbo.GSM, Waarde: "0987654321");

    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo = new(
        Locatie: new Registratiedata.Locatie(
            LocatieId: 1,
            Locatietype.MaatschappelijkeZetelVolgensKbo,
            IsPrimair: false,
            string.Empty,
            new Registratiedata.Adres(
                Straatnaam: "Stationsstraat",
                Huisnummer: "1",
                Busnummer: "B",
                Postcode: "1790",
                Gemeente: "Affligem",
                Land: "België"),
            AdresId: null
        ));

    public readonly ContactgegevenWerdOvergenomenUitKBO TelefoonWerdOvergenomenUitKBO =
        new(ContactgegevenId: 3, Contactgegeventype.Telefoon.Waarde, ContactgegeventypeVolgensKbo.Telefoon, Waarde: "0123456789");

    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new(
        VCode: "V0001014",
        KboNummer: "0987654321",
        Rechtsvorm: "VZW",
        Naam: "Feesten Affligem",
        string.Empty,
        Startdatum: null);

    public readonly ContactgegevenWerdOvergenomenUitKBO WebsiteWerdOvergenomenUitKBO =
        new(ContactgegevenId: 2, Contactgegeventype.Website.Waarde, ContactgegeventypeVolgensKbo.Website,
            Waarde: "https://www.testdata.com");

    public VCode VCode
        => VCode.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
            EmailWerdOvergenomenUitKBO,
            WebsiteWerdOvergenomenUitKBO,
            TelefoonWerdOvergenomenUitKBO,
            GSMWerdOvergenomenUitKBO,
            EmailWerdGewijzigd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001",
               new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(),
               Guid.NewGuid());
}
