namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;

public class V047_AfdelingWerdGeregistreerd_MetBestaandeMoeder_ForNaamWijzigen : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd MoederWerdGeregistreerd;
    public readonly AfdelingWerdGeregistreerd AfdelingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V047_AfdelingWerdGeregistreerd_MetBestaandeMoeder_ForNaamWijzigen()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VCodeMoeder = "V9999017";
        NaamMoeder = "Dee coolste moeder";
        MoederWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCodeMoeder,
            Naam = NaamMoeder,
        };
        KboNummerMoeder = MoederWerdGeregistreerd.KboNummer;

        VCode = "V9999018";
        AfdelingWerdGeregistreerd = fixture.Create<AfdelingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Moedervereniging = new AfdelingWerdGeregistreerd.MoederverenigingsData(KboNummerMoeder, VCodeMoeder, NaamMoeder),
            Locaties = Array.Empty<Registratiedata.Locatie>(),
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<Registratiedata.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<Registratiedata.Vertegenwoordiger>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string KboNummerMoeder { get; set; }

    public string NaamMoeder { get; set; }

    public string VCodeMoeder { get; set; }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { MoederWerdGeregistreerd, AfdelingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
