namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search.DuplicateDetection;

using Events;
using Formatters;
using Schema.Search;
using Vereniging;

public class DuplicateDetectionProjectionHandler
{
    private readonly IElasticRepository _elasticRepository;

    public DuplicateDetectionProjectionHandler(IElasticRepository elasticRepository)
    {
        _elasticRepository = elasticRepository;
    }

    public async Task Handle(EventEnvelope<FeitelijkeVerenigingWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new DuplicateDetectionDocument
            {
                VCode = message.Data.VCode,
                VerenigingsTypeCode = Verenigingstype.FeitelijkeVereniging.Code,
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = message.Data.Locaties.Select(Map).ToArray(),
                HoofdactiviteitVerenigingsloket = MapHoofdactiviteitVerenigingsloket(message.Data.HoofdactiviteitenVerenigingsloket),
                IsVerwijderd = false,
            }
        );

    public async Task Handle(EventEnvelope<AfdelingWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new DuplicateDetectionDocument
            {
                VCode = message.Data.VCode,
                VerenigingsTypeCode = Verenigingstype.Afdeling.Code,
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = message.Data.Locaties.Select(Map).ToArray(),
                HoofdactiviteitVerenigingsloket = MapHoofdactiviteitVerenigingsloket(message.Data.HoofdactiviteitenVerenigingsloket),
                IsVerwijderd = false,
            }
        );

    public async Task Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new DuplicateDetectionDocument
            {
                VCode = message.Data.VCode,
                VerenigingsTypeCode = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = Array.Empty<DuplicateDetectionDocument.Locatie>(),
                HoofdactiviteitVerenigingsloket = Array.Empty<string>(),
                IsVerwijderd = false,
            }
        );

    public async Task Handle(EventEnvelope<NaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new DuplicateDetectionDocument
            {
                Naam = message.Data.Naam,
            }
        );

    public async Task Handle(EventEnvelope<KorteNaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new DuplicateDetectionDocument
            {
                KorteNaam = message.Data.KorteNaam,
            }
        );

    public async Task Handle(EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionDocument
            {
                HoofdactiviteitVerenigingsloket = MapHoofdactiviteitVerenigingsloket(message.Data.HoofdactiviteitenVerenigingsloket),
            }
        );

    public async Task Handle(EventEnvelope<LocatieWerdToegevoegd> message)
        => await _elasticRepository.AppendLocatie<DuplicateDetectionDocument>(message.VCode, Map(message.Data.Locatie));

    public async Task Handle(EventEnvelope<LocatieWerdGewijzigd> message)
        => await _elasticRepository.UpdateLocatie<DuplicateDetectionDocument>(message.VCode, Map(message.Data.Locatie));

    public async Task Handle(EventEnvelope<LocatieWerdVerwijderd> message)
        => await _elasticRepository.RemoveLocatie<DuplicateDetectionDocument>(message.VCode, message.Data.Locatie.LocatieId);

    public async Task Handle(EventEnvelope<VerenigingWerdGestopt> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionDocument
            {
                IsVerwijderd = true,
            }
        );

    private static DuplicateDetectionDocument.Locatie Map(Registratiedata.Locatie locatie)
        => new()
        {
            LocatieId = locatie.LocatieId,
            Locatietype = locatie.Locatietype,
            Naam = locatie.Naam,
            Adresvoorstelling = locatie.Adres.ToAdresString(),
            IsPrimair = locatie.IsPrimair,
            Postcode = locatie.Adres?.Postcode ?? string.Empty,
            Gemeente = locatie.Adres?.Gemeente ?? string.Empty,
        };

    private static string[] MapHoofdactiviteitVerenigingsloket(
        IEnumerable<Registratiedata.HoofdactiviteitVerenigingsloket> hoofdactiviteitenVerenigingsloket)
    {
        return hoofdactiviteitenVerenigingsloket.Select(x => x.Code).ToArray();
    }
}
