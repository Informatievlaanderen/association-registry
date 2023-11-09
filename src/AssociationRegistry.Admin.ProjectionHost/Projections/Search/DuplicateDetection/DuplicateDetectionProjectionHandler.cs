namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search.DuplicateDetection;

using Events;
using Formatters;
using Schema.Search;

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
                Naam = message.Data.Naam,
                                Locaties = message.Data.Locaties.Select(Map).ToArray(),

            }
        );

    public async Task Handle(EventEnvelope<AfdelingWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new DuplicateDetectionDocument
            {
                VCode = message.Data.VCode,
                Naam = message.Data.Naam,
                                Locaties = message.Data.Locaties.Select(Map).ToArray(),

            }
        );

    public async Task Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new DuplicateDetectionDocument
            {
                VCode = message.Data.VCode,
                Naam = message.Data.Naam,
                                Locaties = Array.Empty<VerenigingZoekDocument.Locatie>(),

            }
        );

    public async Task Handle(EventEnvelope<NaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new DuplicateDetectionDocument
            {
                Naam = message.Data.Naam,
                                Locaties = Array.Empty<VerenigingZoekDocument.Locatie>(),

            }
        );

    private static VerenigingZoekDocument.Locatie Map(Registratiedata.Locatie locatie)
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

}
