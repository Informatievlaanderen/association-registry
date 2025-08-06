namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;
using MartenDb.Store;

public class V073_AdresWerdOvergenomenUitAdressenregister : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly List<AdresWerdOvergenomenUitAdressenregister> AdresWerdOvergenomenUitAdressenregisterList;
    public readonly AdresNietUniekInAdressenregister AdresNietUniekInAdressenregister;
    public readonly AdresWerdNietGevondenInAdressenregister AdresWerdNietGevondenInAdressenregister;
    public readonly AdresKonNietOvergenomenWordenUitAdressenregister AdresKonNietOvergenomenWordenUitAdressenregister;
    public readonly LocatieWerdVerwijderd LocatieWerdVerwijderd;
    public readonly CommandMetadata Metadata;

    private class WellKnownLocatieIndices
    {
        public const int EnkelGeregistreerd = 0;
        public const int NietUniek = 1;
        public const int NietGevonden = 2;
        public const int KonNietOvergenomenWorden = 3;
        public const int Verwijderd = 4;
    }

    public V073_AdresWerdOvergenomenUitAdressenregister()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999073";
        Naam = "Dee sjiekste club";

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Locaties = fixture.CreateMany<Registratiedata.Locatie>(5).Select(
                (locatie, w) => locatie with
                {
                    IsPrimair = w == 0,
                    LocatieId = locatie.LocatieId,
                }
            ).ToArray(),
        };

        AdresWerdOvergenomenUitAdressenregisterList = new List<AdresWerdOvergenomenUitAdressenregister>();

        foreach (var locatie in FeitelijkeVerenigingWerdGeregistreerd.Locaties)
        {
            AdresWerdOvergenomenUitAdressenregisterList.Add(
                fixture.Create<AdresWerdOvergenomenUitAdressenregister>()
                    with
                    {
                        VCode = VCode,
                        LocatieId = locatie.LocatieId,
                    });
        }

        AdresNietUniekInAdressenregister = fixture.Create<AdresNietUniekInAdressenregister>() with
        {
            VCode = VCode,
            LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties[WellKnownLocatieIndices.NietUniek].LocatieId,
        };

        AdresWerdNietGevondenInAdressenregister = fixture.Create<AdresWerdNietGevondenInAdressenregister>() with
        {
            VCode = VCode,
            LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties[WellKnownLocatieIndices.NietGevonden].LocatieId,
        };

        AdresKonNietOvergenomenWordenUitAdressenregister = fixture.Create<AdresKonNietOvergenomenWordenUitAdressenregister>() with
        {
            VCode = VCode,
            LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties[WellKnownLocatieIndices.KonNietOvergenomenWorden].LocatieId,
        };

        LocatieWerdVerwijderd = fixture.Create<LocatieWerdVerwijderd>() with
        {
            VCode = VCode,
            Locatie = FeitelijkeVerenigingWerdGeregistreerd.Locaties[WellKnownLocatieIndices.Verwijderd],
        };

        ExpectedLocaties = new[]
        {
            AdresWerdOvergenomenUitAdressenregisterList[WellKnownLocatieIndices.EnkelGeregistreerd],
            AdresWerdOvergenomenUitAdressenregisterList[WellKnownLocatieIndices.KonNietOvergenomenWorden],
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }
    public AdresWerdOvergenomenUitAdressenregister[] ExpectedLocaties { get; set; }

    public IEvent[] GetEvents()
        => new IEvent[]
            {
                FeitelijkeVerenigingWerdGeregistreerd,
            }.Concat(AdresWerdOvergenomenUitAdressenregisterList.ToArray())
             .Concat(new IEvent[]
              {
                  AdresNietUniekInAdressenregister,
                  AdresKonNietOvergenomenWordenUitAdressenregister,
                  AdresWerdNietGevondenInAdressenregister,
                  LocatieWerdVerwijderd,
              })
             .ToArray();

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
