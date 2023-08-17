namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime.Extensions;
using Vereniging;

public class V015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Invalid_Data_Scenario : IScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new(
        "V0001015",
        "0987654321",
        "VZW",
        "Feesten Affligem",
        string.Empty,
        null);

    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo = new(
        Locatie: new Registratiedata.Locatie(
            1,
            Locatietype.MaatschappelijkeZetelVolgensKbo,
            false,
            string.Empty,
            new Registratiedata.Adres(
                "Stationsstraat",
                "1",
                "B",
                "1790",
                "Affligem",
                "België"),
            null
        ));

    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO EmailKonNietOvergenomenWordenUitKbo = new(ContactgegevenType.Email.Waarde,ContactgegevenTypeVolgensKbo.Email.Waarde, "Niet geldig");
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO WebsiteKonNietOvergenomenWordenUitKbo = new(ContactgegevenType.Website.Waarde,ContactgegevenTypeVolgensKbo.Website.Waarde, "ook niet geldig");
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO TelefoonKonNietOvergenomenWordenUitKbo = new(ContactgegevenType.Telefoon.Waarde,ContactgegevenTypeVolgensKbo.Telefoon.Waarde, "n13tg3ld1g");
    public readonly ContactgegevenKonNietOvergenomenWordenUitKBO GSMKonNietOvergenomenWordenUitKbo = new(ContactgegevenType.Telefoon.Waarde,ContactgegevenTypeVolgensKbo.GSM.Waarde, "00kn13tg3ld1g");


    public VCode VCode
        => VCode.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
            EmailKonNietOvergenomenWordenUitKbo,
            WebsiteKonNietOvergenomenWordenUitKbo,
            TelefoonKonNietOvergenomenWordenUitKbo,
            GSMKonNietOvergenomenWordenUitKbo,
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(), Guid.NewGuid());
}
