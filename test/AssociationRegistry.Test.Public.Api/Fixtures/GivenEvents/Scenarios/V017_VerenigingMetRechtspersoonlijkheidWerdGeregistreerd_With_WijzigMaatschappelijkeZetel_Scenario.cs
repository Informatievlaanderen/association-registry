namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime.Extensions;
using System;
using Vereniging;

public class V017_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel_Scenario : IScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new(
        VCode: "V0001017",
        KboNummer: "0987654317",
        Rechtsvorm: "VZW",
        Naam: "Feesten Affligem",
        string.Empty,
        Startdatum: null);

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

    public readonly MaatschappelijkeZetelVolgensKBOWerdGewijzigd MaatschappelijkeZetelVolgensKBOWerdGewijzigd =
        new(LocatieId: 1, Naam: "Station", IsPrimair: true);

    public VCode VCode
        => VCode.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
            MaatschappelijkeZetelVolgensKBOWerdGewijzigd,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001",
               new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(),
               Guid.NewGuid());
}
