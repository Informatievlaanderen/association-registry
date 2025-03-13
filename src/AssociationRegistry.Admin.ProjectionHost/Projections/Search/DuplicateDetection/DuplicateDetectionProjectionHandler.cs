namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search.DuplicateDetection;

using Events;
using Formats;
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
                VerenigingssubtypeCode = null,
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = message.Data.Locaties.Select(Map).ToArray(),
                HoofdactiviteitVerenigingsloket = MapHoofdactiviteitVerenigingsloket(message.Data.HoofdactiviteitenVerenigingsloket),
                IsGestopt = false,
                IsVerwijderd = false,
                IsDubbel = false,
            }
        );

    public async Task Handle(EventEnvelope<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new DuplicateDetectionDocument
            {
                VCode = message.Data.VCode,
                VerenigingsTypeCode = Verenigingstype.VZER.Code,
                VerenigingssubtypeCode = Verenigingssubtype.NietBepaald.Code,
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = message.Data.Locaties.Select(Map).ToArray(),
                HoofdactiviteitVerenigingsloket = MapHoofdactiviteitVerenigingsloket(message.Data.HoofdactiviteitenVerenigingsloket),
                IsGestopt = false,
                IsVerwijderd = false,
                IsDubbel = false,
            }
        );

    public async Task Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new DuplicateDetectionDocument
            {
                VCode = message.Data.VCode,
                VerenigingsTypeCode = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
                VerenigingssubtypeCode = null,
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = Array.Empty<DuplicateDetectionDocument.Locatie>(),
                HoofdactiviteitVerenigingsloket = Array.Empty<string>(),
                IsGestopt = false,
                IsVerwijderd = false,
                IsDubbel = false,
            }
        );

    public async Task Handle(EventEnvelope<NaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new DuplicateDetectionUpdateDocument
            {
                Naam = message.Data.Naam,
            }
        );

    public async Task Handle(EventEnvelope<RechtsvormWerdGewijzigdInKBO> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                VerenigingsTypeCode = Verenigingstype.Parse(message.Data.Rechtsvorm).Code,
            }
        );

    public async Task Handle(EventEnvelope<KorteNaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new DuplicateDetectionUpdateDocument
            {
                KorteNaam = message.Data.KorteNaam,
            }
        );

    public async Task Handle(EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
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
            new DuplicateDetectionUpdateDocument
            {
                IsGestopt = true,
            }
        );

    public async Task Handle(EventEnvelope<VerenigingWerdGestoptInKBO> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                IsGestopt = true,
            }
        );

    public async Task Handle(EventEnvelope<VerenigingWerdVerwijderd> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                IsVerwijderd = true,
            }
        );

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelWerdOvergenomenUitKbo> message)
        => await _elasticRepository.AppendLocatie<DuplicateDetectionDocument>(message.VCode, Map(message.Data.Locatie));

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelWerdGewijzigdInKbo> message)
        => await _elasticRepository.UpdateLocatie<DuplicateDetectionDocument>(message.VCode, Map(message.Data.Locatie));

    public async Task Handle(EventEnvelope<MaatschappelijkeZetelWerdVerwijderdUitKbo> message)
        => await _elasticRepository.RemoveLocatie<DuplicateDetectionDocument>(message.VCode, message.Data.Locatie.LocatieId);

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

    public async Task Handle(EventEnvelope<NaamWerdGewijzigdInKbo> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                Naam = message.Data.Naam,
            }
        );

    public async Task Handle(EventEnvelope<KorteNaamWerdGewijzigdInKbo> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                KorteNaam = message.Data.KorteNaam,
            }
        );

    public async Task Handle(EventEnvelope<AdresWerdOvergenomenUitAdressenregister> message)
        => await _elasticRepository.UpdateLocatie<DuplicateDetectionDocument>(
            message.VCode,
            new DuplicateDetectionDocument.Locatie()
            {
                LocatieId = message.Data.LocatieId,
                Adresvoorstelling = message.Data.Adres.ToAdresString(),
                Postcode = message.Data.Adres?.Postcode ?? string.Empty,
                Gemeente = message.Data.Adres?.Gemeente ?? string.Empty,
            }
        );

    public async Task Handle(EventEnvelope<AdresWerdGewijzigdInAdressenregister> message)
        => await _elasticRepository.UpdateLocatie<DuplicateDetectionDocument>(
            message.VCode,
            new DuplicateDetectionDocument.Locatie()
            {
                LocatieId = message.Data.LocatieId,
                Adresvoorstelling = message.Data.Adres.ToAdresString(),
                Postcode = message.Data.Adres?.Postcode ?? string.Empty,
                Gemeente = message.Data.Adres?.Gemeente ?? string.Empty,
            }
        );

    public async Task Handle(EventEnvelope<LocatieDuplicaatWerdVerwijderdNaAdresMatch> message)
        => await _elasticRepository.RemoveLocatie<DuplicateDetectionDocument>(message.VCode, message.Data.VerwijderdeLocatieId);

    public async Task Handle(EventEnvelope<VerenigingWerdGemarkeerdAlsDubbelVan> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                IsDubbel = true,
            }
        );

    public async Task Handle(EventEnvelope<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                IsDubbel = false,
            }
        );

    public async Task Handle(EventEnvelope<MarkeringDubbeleVerengingWerdGecorrigeerd> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                IsDubbel = false,
            }
        );

   public async Task Handle(EventEnvelope<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                VerenigingsTypeCode = Verenigingstype.VZER.Code,
                VerenigingssubtypeCode = Verenigingssubtype.NietBepaald.Code,
            }
        );

    public async Task Handle(EventEnvelope<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                VerenigingssubtypeCode = Verenigingssubtype.FeitelijkeVereniging.Code,
            }
        );

    public async Task Handle(EventEnvelope<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                VerenigingssubtypeCode = Verenigingssubtype.NietBepaald.Code,
            }
        );

    public async Task Handle(EventEnvelope<VerenigingssubtypeWerdVerfijndNaarSubvereniging> message)
        => await _elasticRepository.UpdateAsync(
            message.VCode,
            new DuplicateDetectionUpdateDocument
            {
                VerenigingssubtypeCode = Verenigingssubtype.Subvereniging.Code,
            }
        );

    private static string[] MapHoofdactiviteitVerenigingsloket(
        IEnumerable<Registratiedata.HoofdactiviteitVerenigingsloket> hoofdactiviteitenVerenigingsloket)
    {
        return hoofdactiviteitenVerenigingsloket.Select(x => x.Code).ToArray();
    }
}
