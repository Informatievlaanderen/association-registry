namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;
using Events.Factories;
using EventStore;
using global::AutoFixture;
using MartenDb.Store;
using Vereniging;

public class V077_LocatieDuplicaatWerdVerwijderdNaAdresMatch : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly AdresWerdOvergenomenUitAdressenregister AdresWerdOvergenomenUitAdressenregister;
    public Registratiedata.Locatie Locatie;
    public readonly LocatieDuplicaatWerdVerwijderdNaAdresMatch LocatieDuplicaatWerdVerwijderdNaAdresMatch;
    public readonly CommandMetadata Metadata;

    public V077_LocatieDuplicaatWerdVerwijderdNaAdresMatch()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999077";
        Naam = "Dee sjiekste club";

        Locatie = fixture.Create<Registratiedata.Locatie>();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Locaties = new List<Registratiedata.Locatie>
            {
                Locatie,
            }.ToArray(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>(),
            Doelgroep = EventFactory.Doelgroep(Doelgroep.Null),
            Vertegenwoordigers = Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Contactgegevens = Array.Empty<Registratiedata.Contactgegeven>(),
        };

        AdresWerdOvergenomenUitAdressenregister = fixture.Create<AdresWerdOvergenomenUitAdressenregister>() with
        {
            LocatieId = Locatie.LocatieId,
            VCode = VCode,
        };

        LocatieDuplicaatWerdVerwijderdNaAdresMatch = fixture.Create<LocatieDuplicaatWerdVerwijderdNaAdresMatch>() with
        {
            VerwijderdeLocatieId = Locatie.LocatieId,
            VCode = VCode,
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public IEvent[] GetEvents()
        => new IEvent[]
           {
               FeitelijkeVerenigingWerdGeregistreerd,
               AdresWerdOvergenomenUitAdressenregister,
           }
          .Concat(new IEvent[]
           {
               LocatieDuplicaatWerdVerwijderdNaAdresMatch,
           })
          .ToArray();

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
