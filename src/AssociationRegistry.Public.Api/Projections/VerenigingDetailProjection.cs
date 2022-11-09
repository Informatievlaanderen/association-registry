namespace AssociationRegistry.Public.Api.Projections;

using System;
using Events;
using Marten.Events.Aggregation;
using Marten.Schema;

public class VerenigingDetailProjection : SingleStreamAggregation<VerenigingDetailDocument>
{
    public VerenigingDetailDocument Create(VerenigingWerdGeregistreerd verenigingWerdGeregistreerd)
        => new()
        {
            VCode = verenigingWerdGeregistreerd.VCode,
            Naam = verenigingWerdGeregistreerd.Naam,
            KorteNaam = verenigingWerdGeregistreerd.KorteNaam,
            KorteBeschrijving = verenigingWerdGeregistreerd.KorteBeschrijving,
            Startdatum = verenigingWerdGeregistreerd.Startdatum,
            KboNummer = verenigingWerdGeregistreerd.KboNummer,
            Status = verenigingWerdGeregistreerd.Status,
            DatumLaatsteAanpassing = verenigingWerdGeregistreerd.DatumLaatsteAanpassing
        };
}

public class VerenigingDetailDocument
{
    [Identity] public string VCode { get; set; } = null!;

    public string Naam { get; set; } = null!;
    public string? KorteNaam { get; set; }
    public string? KorteBeschrijving { get; set; }
    public DateOnly? Startdatum { get; set; }
    public string? KboNummer { get; set; }
    public string Status { get; set; } = null!;
    public DateOnly DatumLaatsteAanpassing { get; set; }
}
