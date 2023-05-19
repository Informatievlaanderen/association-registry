namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System.Linq;
using Events;
using Infrastructure.Extensions;
using Marten.Events;
using Schema.Search;
using Vereniging;

public class ElasticEventProjection
{
    private readonly IVerenigingBrolFeeder _brolFeeder;

    public ElasticEventProjection(IVerenigingBrolFeeder brolFeeder)
    {
        _brolFeeder = brolFeeder;
    }

    public VerenigingDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> message)
        => new()
        {
            VCode = message.Data.VCode,
            Type = new VerenigingDocument.VerenigingsType
            {
                Code = VerenigingsType.FeitelijkeVereniging.Code,
                Beschrijving = VerenigingsType.FeitelijkeVereniging.Beschrijving,
            },
            Naam = message.Data.Naam,
            KorteNaam = message.Data.KorteNaam,
            Locaties = message.Data.Locaties.Select(
                loc => new VerenigingDocument.Locatie
                {
                    Locatietype = loc.Locatietype,
                    Naam = loc.Naam,
                    Adres = loc.ToAdresString(),
                    Hoofdlocatie = loc.Hoofdlocatie,
                    Postcode = loc.Postcode,
                    Gemeente = loc.Gemeente,
                }).ToArray(),
            HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
                .Select(
                    hoofdactiviteitVerenigingsloket =>
                        new VerenigingDocument.HoofdactiviteitVerenigingsloket
                        {
                            Code = hoofdactiviteitVerenigingsloket.Code,
                            Naam = hoofdactiviteitVerenigingsloket.Beschrijving,
                        })
                .ToArray(),
            Doelgroep = _brolFeeder.Doelgroep,
            Activiteiten = _brolFeeder.Activiteiten.ToArray(),
        };

    public VerenigingDocument Create(IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message)
        => new()
        {
            VCode = message.Data.VCode,
            Type = new VerenigingDocument.VerenigingsType { Code = VerenigingsType.VerenigingMetRechtspersoonlijkheid.Code, Beschrijving = VerenigingsType.VerenigingMetRechtspersoonlijkheid.Beschrijving },
            Naam = message.Data.Naam,
            KorteNaam = message.Data.KorteNaam,
            Locaties = Array.Empty<VerenigingDocument.Locatie>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<VerenigingDocument.HoofdactiviteitVerenigingsloket>(),
            Doelgroep = "",
            Activiteiten = Array.Empty<string>(),
            Sleutels = new[]
            {
                new VerenigingDocument.Sleutel
                {
                    Bron = Bron.Kbo,
                    Waarde = message.Data.KboNummer,
                },
            },
        };

    public void Apply(IEvent<NaamWerdGewijzigd> message, VerenigingDocument document)
        => document.Naam = message.Data.Naam;

    public void Apply(IEvent<KorteNaamWerdGewijzigd> message, VerenigingDocument document)
        => document.KorteNaam = message.Data.KorteNaam;

    public void Apply(IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message, VerenigingDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = message.Data.HoofdactiviteitenVerenigingsloket
            .Select(
                hoofdactiviteitVerenigingsloket => new VerenigingDocument.HoofdactiviteitVerenigingsloket
                {
                    Code = hoofdactiviteitVerenigingsloket.Code,
                    Naam = hoofdactiviteitVerenigingsloket.Beschrijving,
                })
            .ToArray();
    }
}
