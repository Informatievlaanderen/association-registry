namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime.Extensions;
using Vereniging;

public class V0015_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_Adres_Scenario : IScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new(
        "V0001015",
        "0000000000",
        "VZW",
        "VZW 0000000000",
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

    public VCode VCode
        => VCode.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelWerdOvergenomenUitKbo
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new("OVO000001", new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(), Guid.NewGuid());
}
