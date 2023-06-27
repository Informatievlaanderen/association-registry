namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

using System;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Formatters;
using Vereniging;
using Schema.Search;

public class ElasticEventHandler
{
    private readonly IElasticRepository _elasticRepository;
    private readonly IVerenigingBrolFeeder _brolFeeder;

    public ElasticEventHandler(IElasticRepository elasticRepository, IVerenigingBrolFeeder brolFeeder)
    {
        _elasticRepository = elasticRepository;
        _brolFeeder = brolFeeder;
    }

    public async Task Handle(EventEnvelope<FeitelijkeVerenigingWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingZoekDocument
            {
                VCode = message.Data.VCode,
                Type = new VerenigingZoekDocument.VerenigingsType
                {
                    Code = Verenigingstype.FeitelijkeVereniging.Code,
                    Beschrijving = Verenigingstype.FeitelijkeVereniging.Beschrijving,
                },
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = message.Data.Locaties.Select(
                    loc => new VerenigingZoekDocument.Locatie
                    {
                        Locatietype = loc.Locatietype,
                        Naam = loc.Naam,
                        Adresvoorstelling = loc.Adres.ToAdresString(),
                        IsPrimair = loc.IsPrimair,
                        Postcode = loc.Adres?.Postcode ?? string.Empty,
                        Gemeente = loc.Adres?.Gemeente ?? string.Empty,
                    }).ToArray(),
                IsUitgeschrevenUitPubliekeDatastroom = message.Data.IsUitgeschrevenUitPubliekeDatastroom,
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                    .Select(
                        hoofdactiviteitVerenigingsloket =>
                            new VerenigingZoekDocument.HoofdactiviteitVerenigingsloket
                            {
                                Code = hoofdactiviteitVerenigingsloket.Code,
                                Naam = hoofdactiviteitVerenigingsloket.Beschrijving,
                            })
                    .ToArray(),
                Doelgroep = _brolFeeder.Doelgroep,
                Activiteiten = _brolFeeder.Activiteiten.ToArray(),
            }
        );

    public async Task Handle(EventEnvelope<AfdelingWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingZoekDocument
            {
                VCode = message.Data.VCode,
                Type = new VerenigingZoekDocument.VerenigingsType
                {
                    Code = Verenigingstype.Afdeling.Code,
                    Beschrijving = Verenigingstype.Afdeling.Beschrijving,
                },
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = message.Data.Locaties.Select(
                    loc => new VerenigingZoekDocument.Locatie
                    {
                        Locatietype = loc.Locatietype,
                        Naam = loc.Naam,
                        Adresvoorstelling = loc.Adres.ToAdresString(),
                        IsPrimair = loc.IsPrimair,
                        Postcode = loc.Adres?.Postcode ?? string.Empty,
                        Gemeente = loc.Adres?.Gemeente ?? string.Empty,
                    }).ToArray(),
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                    .Select(
                        hoofdactiviteitVerenigingsloket =>
                            new VerenigingZoekDocument.HoofdactiviteitVerenigingsloket
                            {
                                Code = hoofdactiviteitVerenigingsloket.Code,
                                Naam = hoofdactiviteitVerenigingsloket.Beschrijving,
                            })
                    .ToArray(),
                IsUitgeschrevenUitPubliekeDatastroom = false,
                Doelgroep = _brolFeeder.Doelgroep,
                Activiteiten = _brolFeeder.Activiteiten.ToArray(),
                Sleutels = Array.Empty<VerenigingZoekDocument.Sleutel>(),
            }
        );

    public async Task Handle(EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message)
        => await _elasticRepository.IndexAsync(
            new VerenigingZoekDocument
            {
                VCode = message.Data.VCode,
                Type = new VerenigingZoekDocument.VerenigingsType { Code = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Code, Beschrijving = Verenigingstype.VerenigingMetRechtspersoonlijkheid.Beschrijving },
                Naam = message.Data.Naam,
                KorteNaam = message.Data.KorteNaam,
                Locaties = Array.Empty<VerenigingZoekDocument.Locatie>(),
                HoofdactiviteitenVerenigingsloket = Array.Empty<VerenigingZoekDocument.HoofdactiviteitVerenigingsloket>(),
                Doelgroep = "",
                Activiteiten = Array.Empty<string>(),
                Sleutels = new[]
                {
                    new VerenigingZoekDocument.Sleutel
                    {
                        Bron = Verenigingsbron.Kbo,
                        Waarde = message.Data.KboNummer,
                    },
                },
            }
        );

    public async Task Handle(EventEnvelope<NaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new VerenigingZoekDocument
            {
                Naam = message.Data.Naam,
            }
        );

    public async Task Handle(EventEnvelope<KorteNaamWerdGewijzigd> message)
        => await _elasticRepository.UpdateAsync(
            message.Data.VCode,
            new VerenigingZoekDocument
            {
                KorteNaam = message.Data.KorteNaam,
            }
        );

    public void Handle(EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message)
    {
        _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                    .Select(
                        hoofdactiviteitVerenigingsloket =>
                            new VerenigingZoekDocument.HoofdactiviteitVerenigingsloket
                            {
                                Code = hoofdactiviteitVerenigingsloket.Code,
                                Naam = hoofdactiviteitVerenigingsloket.Beschrijving,
                            })
                    .ToArray(),
            });
    }

    public void Handle(EventEnvelope<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> message)
    {
        _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                IsUitgeschrevenUitPubliekeDatastroom = true,
            });
    }

    public void Handle(EventEnvelope<VerenigingWerdIngeschrevenInPubliekeDatastroom> message)
    {
        _elasticRepository.UpdateAsync(
            message.VCode,
            new VerenigingZoekDocument
            {
                IsUitgeschrevenUitPubliekeDatastroom = false,
            });
    }

    public void Handle(EventEnvelope<LocatieWerdToegevoegd> message)
    {
        _elasticRepository.AppendLocatie(
            message.VCode,
            Map(message.Data.Locatie));
    }

    private static VerenigingZoekDocument.Locatie Map(Registratiedata.Locatie locatie)
        => new()
        {
            Locatietype = locatie.Locatietype,
            Naam = locatie.Naam,
            Adresvoorstelling = locatie.Adres.ToAdresString(),
            IsPrimair = locatie.IsPrimair,
            Postcode = locatie.Adres?.Postcode ?? string.Empty,
            Gemeente = locatie.Adres?.Gemeente ?? string.Empty,
        };
}
