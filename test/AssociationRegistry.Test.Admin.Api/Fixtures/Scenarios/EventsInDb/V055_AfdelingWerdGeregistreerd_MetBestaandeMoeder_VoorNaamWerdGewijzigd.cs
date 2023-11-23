namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;
using Vereniging;

public class V055_AfdelingWerdGeregistreerd_MetBestaandeMoeder_VoorNaamWerdGewijzigd : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd MoederWerdGeregistreerd;
    public readonly AfdelingWerdGeregistreerd AfdelingWerdGeregistreerd;
    public readonly NaamWerdGewijzigd NaamWerdGewijzigd;
    public readonly CommandMetadata Metadata;

    public V055_AfdelingWerdGeregistreerd_MetBestaandeMoeder_VoorNaamWerdGewijzigd()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VCodeMoeder = "V9999055";
        NaamMoeder = "De coolste moeder";

        VCode = "V9999056";
        NaamAfdeling = "De coolste afdeling";

        MoederWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCodeMoeder,
            Naam = NaamMoeder,
            Rechtsvorm = "SVON"
        };
        KboNummerMoeder = MoederWerdGeregistreerd.KboNummer;

        AfdelingWerdGeregistreerd = fixture.Create<AfdelingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Moedervereniging = new AfdelingWerdGeregistreerd.MoederverenigingsData(KboNummerMoeder, VCodeMoeder, NaamMoeder),
            Locaties = Array.Empty<Registratiedata.Locatie>(),
            Naam = "De minder coole afdeling",
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<Registratiedata.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<Registratiedata.Vertegenwoordiger>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>(),
        };

        NaamWerdGewijzigd = fixture.Create<NaamWerdGewijzigd>() with
        {
            VCode = VCode,
            Naam = NaamAfdeling
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string KboNummerMoeder { get; set; }

    public string NaamMoeder { get; set; }

    public string NaamAfdeling { get; set; }

    public string VCodeMoeder { get; set; }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { MoederWerdGeregistreerd, AfdelingWerdGeregistreerd, NaamWerdGewijzigd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
