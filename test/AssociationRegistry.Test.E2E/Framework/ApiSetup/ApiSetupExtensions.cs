namespace AssociationRegistry.Test.E2E.Framework.ApiSetup;

using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Bewaartermijnen;
using Events;
using JasperFx.Events;
using Marten;

public static class ApiSetupExtensions
{
    public static async Task<long> WaitForAdresMatchEventForEachLocation(
        this IApiSetup apiSetup,
        string vCode,
        int locationsCount
    )
    {
        await using var session = apiSetup.AdminProjectionHost.DocumentStore().LightweightSession();
        var events = await session.Events.FetchStreamAsync(vCode);

        var counter = 0;

        while (!HasForeachLocationAnAdresMatchEvent(locationsCount, events))
        {
            await Task.Delay(200);
            events = await session.Events.FetchStreamAsync(vCode);

            if (++counter > 50)
                throw new Exception(
                    $"Kept waiting for Adresmatch... Events committed: {string.Join(separator: ", ", events.Select(x => x.EventTypeName))}"
                );
        }

        return events.Max(a => a.Sequence);
    }

    public static async Task WaitForBewaartermijnEvent(this IApiSetup apiSetup, string vCode, int entityId)
    {
        await using var session = apiSetup.AdminApiHost.DocumentStore().LightweightSession();
        var events = await session.Events.QueryRawEventDataOnly<BewaartermijnWerdGestartV2>().ToListAsync();
        var counter = 0;

        while (!HasBewaartermijnEvent(events, vCode, entityId))
        {
            await Task.Delay(200);
            events = await session.Events.QueryRawEventDataOnly<BewaartermijnWerdGestartV2>().ToListAsync();

            if (++counter > 75)
                throw new Exception($"Kept waiting for Bewaartermijn event ...");
        }
    }

    private static bool HasBewaartermijnEvent(
        IReadOnlyList<BewaartermijnWerdGestartV2> events,
        string vCode,
        int entityId
    )
    {
        var bewaartermijnEvent = events.FirstOrDefault(x =>
            x.BewaartermijnId
            == BewaartermijnId.CreateId(VCode.Create(vCode), PersoonsgegevensType.Vertegenwoordigers, entityId)
        );

        return bewaartermijnEvent != null;
    }

    private static bool HasForeachLocationAnAdresMatchEvent(
        int aantalLocaties,
        IReadOnlyList<JasperFx.Events.IEvent> events
    )
    {
        var adresEventCount = events.Count(e =>
            e.EventType == typeof(AdresWerdOvergenomenUitAdressenregister)
            || e.EventType == typeof(AdresKonNietOvergenomenWordenUitAdressenregister)
            || e.EventType == typeof(AdresWerdNietGevondenInAdressenregister)
            || e.EventType == typeof(NietUniekeAdresMatchUitAdressenregister)
            || e.EventType == typeof(AdresHeeftGeenVerschillenMetAdressenregister)
        );
        return adresEventCount == aantalLocaties;
    }

    private static Type[] AdresEventsTypes =>
        [
            typeof(Event<AdresWerdOvergenomenUitAdressenregister>),
            typeof(Event<AdresKonNietOvergenomenWordenUitAdressenregister>),
            typeof(Event<AdresWerdNietGevondenInAdressenregister>),
            typeof(Event<AdresNietUniekInAdressenregister>),
            typeof(Event<AdresHeeftGeenVerschillenMetAdressenregister>),
        ];
}
