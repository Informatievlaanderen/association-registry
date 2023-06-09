namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using System;
using System.Linq;
using System.Threading.Tasks;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Marten;
using Marten.Events;
using Marten.Events.Projections;

public class BeheerVerenigingDetailProjection : EventProjection
{
    public BeheerVerenigingDetailProjection()
    {
        // Needs a batch size of 1, because otherwise if Registered and NameChanged arrive in 1 batch/slice,
        // the newly persisted document from xxxWerdGeregistreerd is not in the
        // Query yet when we handle NaamWerdGewijzigd.
        // see also https://martendb.io/events/projections/event-projections.html#reusing-documents-in-the-same-batch
        Options.BatchSize = 1;
    }

    public BeheerVerenigingDetailDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Type = BeheerVerenigingDetailMapper.MapVerenigingsType(Verenigingstype.FeitelijkeVereniging),
            Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum?.ToString(Schema.WellknownFormats.DateOnly),
            DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)),
            Status = "Actief",
            Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Data.Contactgegevens
                .Select(BeheerVerenigingDetailMapper.MapContactgegeven)
                .ToArray(),
            Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties.Select(BeheerVerenigingDetailMapper.MapLocatie).ToArray(),
            Vertegenwoordigers = feitelijkeVerenigingWerdGeregistreerd.Data.Vertegenwoordigers
                .Select(BeheerVerenigingDetailMapper.MapVertegenwoordiger)
                .ToArray(),
            HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data
                .HoofdactiviteitenVerenigingsloket.Select(BeheerVerenigingDetailMapper.MapHoofdactiviteitVerenigingsloket)
                .ToArray(),
            Metadata = new Metadata(feitelijkeVerenigingWerdGeregistreerd.Sequence, feitelijkeVerenigingWerdGeregistreerd.Version),
        };

    public async Task Project(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd, IDocumentOperations ops)
    {
        var afdeling = BeheerVerenigingDetailProjector.Create(afdelingWerdGeregistreerd);

        ops.Insert(afdeling);

        if (string.IsNullOrEmpty(afdelingWerdGeregistreerd.Data.Moedervereniging.VCode))
            return;

        var moeder = ops.Load<BeheerVerenigingDetailDocument>(afdelingWerdGeregistreerd.Data.Moedervereniging.VCode);

        moeder = BeheerVerenigingDetailProjector.Apply(afdelingWerdGeregistreerd, moeder);

        ops.Store(moeder);
    }

    public BeheerVerenigingDetailDocument Create(IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
        => new()
        {
            VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
            Type = new BeheerVerenigingDetailDocument.VerenigingsType
            {
                Code = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Code,
                Beschrijving = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Beschrijving,
            },
            Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
            KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = string.Empty,
            Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum?.ToString(Schema.WellknownFormats.DateOnly),
            Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
            DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)),
            Status = "Actief",
            Contactgegevens = Array.Empty<BeheerVerenigingDetailDocument.Contactgegeven>(),
            Locaties = Array.Empty<BeheerVerenigingDetailDocument.Locatie>(),
            Vertegenwoordigers = Array.Empty<BeheerVerenigingDetailDocument.Vertegenwoordiger>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket>(),
            Sleutels = new[]
            {
                BeheerVerenigingDetailMapper.MapKboSleutel(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer),
            },
            Metadata = new Metadata(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence, verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version),
        };

    public async Task Project(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(naamWerdGewijzigd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(naamWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(korteNaamWerdGewijzigd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(korteNaamWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(korteBeschrijvingWerdGewijzigd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(korteBeschrijvingWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(startdatumWerdGewijzigd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(startdatumWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(contactgegevenWerdToegevoegd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdToegevoegd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(contactgegevenWerdGewijzigd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(contactgegevenWerdVerwijderd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(contactgegevenWerdVerwijderd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofactiviteitenVerenigingloketWerdenGewijzigd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(hoofactiviteitenVerenigingloketWerdenGewijzigd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(hoofactiviteitenVerenigingloketWerdenGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(vertegenwoordigerWerdToegevoegd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdToegevoegd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdGewijzigd> vertegenwoordigerWerdGewijzigd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(vertegenwoordigerWerdGewijzigd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdGewijzigd, doc);

        ops.Store(doc);
    }

    public async Task Project(IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd, IDocumentOperations ops)
    {
        var doc = ops.Load<BeheerVerenigingDetailDocument>(vertegenwoordigerWerdVerwijderd.StreamKey!);

        BeheerVerenigingDetailProjector.Apply(vertegenwoordigerWerdVerwijderd, doc);

        ops.Store(doc);
    }
}
