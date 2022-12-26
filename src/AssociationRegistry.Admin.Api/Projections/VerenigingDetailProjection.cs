﻿namespace AssociationRegistry.Admin.Api.Projections;

using System;
using System.Linq;
using Extensions;
using Framework;
using Vereniging;
using Infrastructure;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;

public record Metadata(long Sequence);

public class VerenigingDetailProjection : SingleStreamAggregation<VerenigingDetailDocument>
{
    public VerenigingDetailDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
        => new()
        {
            VCode = verenigingWerdGeregistreerd.Data.VCode,
            Naam = verenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = verenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = verenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = verenigingWerdGeregistreerd.Data.Startdatum,
            KboNummer = verenigingWerdGeregistreerd.Data.KboNummer,
            DatumLaatsteAanpassing = verenigingWerdGeregistreerd.Data.DatumLaatsteAanpassing ?? DateOnly.Parse(verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Tijdstip)),
            Status = "Actief",
            Contacten = verenigingWerdGeregistreerd.Data.Contacten?.Select(
                            c => new VerenigingDetailDocument.ContactInfo()
                            {
                                Contactnaam = c.Contactnaam,
                                Email = c.Email,
                                Telefoon = c.Telefoon,
                                Website = c.Website,
                                SocialMedia = c.SocialMedia,
                            }).ToArray()
                        ?? Array.Empty<VerenigingDetailDocument.ContactInfo>(),
            Locaties = verenigingWerdGeregistreerd.Data.Locaties?.Select(MapLocatie).ToArray() ?? Array.Empty<VerenigingDetailDocument.Locatie>(),
            Metadata = new Metadata(verenigingWerdGeregistreerd.Sequence)
        };

    private static VerenigingDetailDocument.Locatie MapLocatie(VerenigingWerdGeregistreerd.Locatie loc)
        => new()
        {
            Hoofdlocatie = loc.HoofdLocatie,
            Naam = loc.Naam,
            Type = loc.LocatieType,
            Straatnaam = loc.Straatnaam,
            Huisnummer = loc.Huisnummer,
            Busnummer = loc.Busnummer,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
            Land = loc.Land,
            Adres = loc.ToAdresString(),
        };
}

public class VerenigingDetailDocument : IVCode, IMetadata
{
    [Identity] public string VCode { get; set; } = null!;

    public string Naam { get; set; } = null!;
    public string? KorteNaam { get; set; }
    public string? KorteBeschrijving { get; set; }
    public DateOnly? Startdatum { get; set; }
    public string? KboNummer { get; set; }
    public string Status { get; set; } = null!;
    public DateOnly DatumLaatsteAanpassing { get; set; }
    public Locatie[] Locaties { get; set; } = null!;
    public ContactInfo[] Contacten { get; set; } = Array.Empty<ContactInfo>();
    public Metadata Metadata { get; set; } = null!;

    public class ContactInfo
    {
        public string? Contactnaam { get; set; }
        public string? Email { get; set; }
        public string? Telefoon { get; set; }
        public string? Website { get; set; }
        public string? SocialMedia { get; set; }
    }

    public class Locatie
    {
        public string Type { get; set; } = null!;

        public bool Hoofdlocatie { get; set; }

        public string Adres { get; set; } = null!;
        public string? Naam { get; set; }

        public string Straatnaam { get; set; } = null!;

        public string Huisnummer { get; set; } = null!;

        public string? Busnummer { get; set; }

        public string Postcode { get; set; } = null!;

        public string Gemeente { get; set; } = null!;

        public string Land { get; set; } = null!;
    }
}
