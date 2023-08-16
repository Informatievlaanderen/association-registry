namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime.Extensions;
using Vereniging;

public class V014_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data_Scenario : IScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new(
        "V0001014",
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

    public readonly ContactgegevenWerdOvergenomenUitKBO EmailWerdOvergenomenUitKBO = new(1, ContactgegevenType.Email.Waarde, ContactgegevenTypeVolgensKbo.Email, "email@testdata.com");
    public readonly ContactgegevenWerdOvergenomenUitKBO WebsiteWerdOvergenomenUitKBO = new(2, ContactgegevenType.Website.Waarde, ContactgegevenTypeVolgensKbo.Website, "https://www.testdata.com");
    public readonly ContactgegevenWerdOvergenomenUitKBO TelefoonWerdOvergenomenUitKBO = new(3, ContactgegevenType.Telefoon.Waarde, ContactgegevenTypeVolgensKbo.Telefoon, "0123456789");
    public readonly ContactgegevenWerdOvergenomenUitKBO GSMWerdOvergenomenUitKBO = new(4, ContactgegevenType.Telefoon.Waarde, ContactgegevenTypeVolgensKbo.GSM, "0987654321");


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
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(), Guid.NewGuid());
}
