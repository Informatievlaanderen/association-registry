namespace AssociationRegistry.Admin.Api.Projections.Detail;

using System;
using System.Linq;
using System.Threading.Tasks;
using Constants;
using Events;
using Framework;
using Infrastructure.Extensions;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Vereniging;

public record Metadata(long Sequence, long Version);

public class BeheerVerenigingDetailProjection : EventProjection
{
    public BeheerVerenigingDetailDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Type = BeheerVerenigingDetailMapper.MapVerenigingsType(Verenigingstype.FeitelijkeVereniging),
            Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
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

    public BeheerVerenigingDetailDocument Create(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd)
        => new()
        {
            VCode = afdelingWerdGeregistreerd.Data.VCode,
            Type = BeheerVerenigingDetailMapper.MapVerenigingsType(Verenigingstype.Afdeling),
            Naam = afdelingWerdGeregistreerd.Data.Naam,
            KorteNaam = afdelingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = afdelingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = afdelingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            DatumLaatsteAanpassing = afdelingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = "Actief",
            Contactgegevens = afdelingWerdGeregistreerd.Data.Contactgegevens.Select(
                BeheerVerenigingDetailMapper.MapContactgegeven)
                .ToArray(),
            Locaties = afdelingWerdGeregistreerd.Data.Locaties.Select(BeheerVerenigingDetailMapper.MapLocatie)
                .ToArray(),
            Vertegenwoordigers = afdelingWerdGeregistreerd.Data.Vertegenwoordigers.Select(
                BeheerVerenigingDetailMapper.MapVertegenwoordiger)
                .ToArray(),
            HoofdactiviteitenVerenigingsloket = afdelingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
                BeheerVerenigingDetailMapper.MapHoofdactiviteitVerenigingsloket)
                .ToArray(),
            Relaties = new[]
            {
                BeheerVerenigingDetailMapper.MapMoederRelatie(afdelingWerdGeregistreerd.Data.Moedervereniging),
            },
            Metadata = new Metadata(afdelingWerdGeregistreerd.Sequence, afdelingWerdGeregistreerd.Version),
        };

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
            Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
            DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
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
        var doc = ops.Load<BeheerVerenigingDetailDocument>(naamWerdGewijzigd.Data.VCode);

        BeheerVerenigingDetailProjector.Apply(naamWerdGewijzigd, doc);

        ops.Store(doc);
    }
}
