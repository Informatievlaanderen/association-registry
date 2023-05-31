namespace AssociationRegistry.Admin.Api.Projections.Detail;

using System;
using System.Linq;
using Constants;
using Events;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;
using Vereniging;

public record Metadata(long Sequence, long Version);

public class BeheerVerenigingDetailProjection : SingleStreamAggregation<BeheerVerenigingDetailDocument>
{
    public BeheerVerenigingDetailDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Type = new BeheerVerenigingDetailDocument.VerenigingsType
            {
                Code = Verenigingstype.FeitelijkeVereniging.Code,
                Beschrijving = Verenigingstype.FeitelijkeVereniging.Beschrijving,
            },
            Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = "Actief",
            Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Data.Contactgegevens.Select(
                c => new BeheerVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = c.ContactgegevenId,
                    Type = c.Type.ToString(),
                    Waarde = c.Waarde,
                    Beschrijving = c.Beschrijving,
                    IsPrimair = c.IsPrimair,
                }).ToArray(),
            Locaties = ToLocationArray(feitelijkeVerenigingWerdGeregistreerd.Data.Locaties),
            Vertegenwoordigers = feitelijkeVerenigingWerdGeregistreerd.Data.Vertegenwoordigers.Select(
                v => new BeheerVerenigingDetailDocument.Vertegenwoordiger
                {
                    VertegenwoordigerId = v.VertegenwoordigerId,
                    IsPrimair = v.IsPrimair,
                    Roepnaam = v.Roepnaam,
                    Rol = v.Rol,
                    Achternaam = v.Achternaam,
                    Voornaam = v.Voornaam,
                    Email = v.Email,
                    Telefoon = v.Telefoon,
                    Mobiel = v.Mobiel,
                    SocialMedia = v.SocialMedia,
                }).ToArray(),
            HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
                h => new BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
                {
                    Code = h.Code,
                    Beschrijving = h.Beschrijving,
                }).ToArray(),
            Metadata = new Metadata(feitelijkeVerenigingWerdGeregistreerd.Sequence, feitelijkeVerenigingWerdGeregistreerd.Version),
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
            Sleutels = new BeheerVerenigingDetailDocument.Sleutel[]
            {
                new()
                {
                    Bron = Bron.Kbo.Waarde,
                    Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                },
            },
            Metadata = new Metadata(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Sequence, verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Version),
        };

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
        document.DatumLaatsteAanpassing = naamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(naamWerdGewijzigd.Sequence, naamWerdGewijzigd.Version);
    }

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;
        document.DatumLaatsteAanpassing = korteNaamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(korteNaamWerdGewijzigd.Sequence, korteNaamWerdGewijzigd.Version);
    }

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
        document.DatumLaatsteAanpassing = korteBeschrijvingWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(korteBeschrijvingWerdGewijzigd.Sequence, korteBeschrijvingWerdGewijzigd.Version);
    }

    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Startdatum = startdatumWerdGewijzigd.Data.Startdatum?.ToString(WellknownFormats.DateOnly);
        document.DatumLaatsteAanpassing = startdatumWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(startdatumWerdGewijzigd.Sequence, startdatumWerdGewijzigd.Version);
    }

    public void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, BeheerVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.Append(
            new BeheerVerenigingDetailDocument.Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                Type = contactgegevenWerdToegevoegd.Data.Type,
                Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
            }).ToArray();
        document.DatumLaatsteAanpassing = contactgegevenWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(contactgegevenWerdToegevoegd.Sequence, contactgegevenWerdToegevoegd.Version);
    }

    public void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
            .Where(c => c.ContactgegevenId != contactgegevenWerdGewijzigd.Data.ContactgegevenId)
            .Append(
                new BeheerVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                    Type = contactgegevenWerdGewijzigd.Data.Type,
                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                })
            .ToArray();

        document.DatumLaatsteAanpassing = contactgegevenWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(contactgegevenWerdGewijzigd.Sequence, contactgegevenWerdGewijzigd.Version);
    }

    public void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, BeheerVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
            .Where(
                c => c.ContactgegevenId != contactgegevenWerdVerwijderd.Data.ContactgegevenId)
            .ToArray();

        document.DatumLaatsteAanpassing = contactgegevenWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(contactgegevenWerdVerwijderd.Sequence, contactgegevenWerdVerwijderd.Version);
    }

    public void Apply(IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofactiviteitenVerenigingloketWerdenGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = hoofactiviteitenVerenigingloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket.Select(
            h => new BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
            {
                Code = h.Code,
                Beschrijving = h.Beschrijving,
            }).ToArray();
        document.DatumLaatsteAanpassing = hoofactiviteitenVerenigingloketWerdenGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(hoofactiviteitenVerenigingloketWerdenGewijzigd.Sequence, hoofactiviteitenVerenigingloketWerdenGewijzigd.Version);
    }

    public void Apply(IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd, BeheerVerenigingDetailDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers.Append(
            new BeheerVerenigingDetailDocument.Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId,
                Achternaam = vertegenwoordigerWerdToegevoegd.Data.Achternaam,
                Voornaam = vertegenwoordigerWerdToegevoegd.Data.Voornaam,
                Roepnaam = vertegenwoordigerWerdToegevoegd.Data.Roepnaam,
                Rol = vertegenwoordigerWerdToegevoegd.Data.Rol,
                IsPrimair = vertegenwoordigerWerdToegevoegd.Data.IsPrimair,
                Email = vertegenwoordigerWerdToegevoegd.Data.Email,
                Telefoon = vertegenwoordigerWerdToegevoegd.Data.Telefoon,
                Mobiel = vertegenwoordigerWerdToegevoegd.Data.Mobiel,
                SocialMedia = vertegenwoordigerWerdToegevoegd.Data.SocialMedia,
            }).ToArray();

        document.DatumLaatsteAanpassing = vertegenwoordigerWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(vertegenwoordigerWerdToegevoegd.Sequence, vertegenwoordigerWerdToegevoegd.Version);
    }

    public void Apply(IEvent<VertegenwoordigerWerdGewijzigd> vertegenwoordigerWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        var vertegenwoordigerToUpdate = document.Vertegenwoordigers.Single(v => v.VertegenwoordigerId == vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId);
        document.Vertegenwoordigers = document.Vertegenwoordigers
            .Where(c => c.VertegenwoordigerId != vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId)
            .Append(
                new BeheerVerenigingDetailDocument.Vertegenwoordiger
                {
                    VertegenwoordigerId = vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId,
                    Achternaam = vertegenwoordigerToUpdate.Achternaam,
                    Voornaam = vertegenwoordigerToUpdate.Voornaam,
                    Roepnaam = vertegenwoordigerWerdGewijzigd.Data.Roepnaam,
                    Rol = vertegenwoordigerWerdGewijzigd.Data.Rol,
                    IsPrimair = vertegenwoordigerWerdGewijzigd.Data.IsPrimair,
                    Email = vertegenwoordigerWerdGewijzigd.Data.Email,
                    Telefoon = vertegenwoordigerWerdGewijzigd.Data.Telefoon,
                    Mobiel = vertegenwoordigerWerdGewijzigd.Data.Mobiel,
                    SocialMedia = vertegenwoordigerWerdGewijzigd.Data.SocialMedia,
                })
            .ToArray();

        document.DatumLaatsteAanpassing = vertegenwoordigerWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(vertegenwoordigerWerdGewijzigd.Sequence, vertegenwoordigerWerdGewijzigd.Version);
    }

    public void Apply(IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd, BeheerVerenigingDetailDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers
            .Where(
                c => c.VertegenwoordigerId != vertegenwoordigerWerdVerwijderd.Data.VertegenwoordigerId)
            .ToArray();

        document.DatumLaatsteAanpassing = vertegenwoordigerWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = new Metadata(vertegenwoordigerWerdVerwijderd.Sequence, vertegenwoordigerWerdVerwijderd.Version);
    }

    private static BeheerVerenigingDetailDocument.Locatie[] ToLocationArray(FeitelijkeVerenigingWerdGeregistreerd.Locatie[]? locaties)
        => locaties?.Select(MapLocatie).ToArray() ?? Array.Empty<BeheerVerenigingDetailDocument.Locatie>();

    private static BeheerVerenigingDetailDocument.Locatie MapLocatie(FeitelijkeVerenigingWerdGeregistreerd.Locatie loc)
        => new()
        {
            Hoofdlocatie = loc.Hoofdlocatie,
            Naam = loc.Naam,
            Locatietype = loc.Locatietype,
            Straatnaam = loc.Straatnaam,
            Huisnummer = loc.Huisnummer,
            Busnummer = loc.Busnummer,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
            Land = loc.Land,
            Adres = loc.ToAdresString(),
        };
}

public record BeheerVerenigingDetailDocument : IVCode, IMetadata
{
    [Identity] public string VCode { get; init; } = null!;
    public string Naam { get; set; } = null!;
    public VerenigingsType Type { get; set; } = null!;
    public string? KorteNaam { get; set; }
    public string? KorteBeschrijving { get; set; }
    public string? Startdatum { get; set; }

    public string? Rechtsvorm { get; set; }
    public string Status { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = Array.Empty<Locatie>();
    public Contactgegeven[] Contactgegevens { get; set; } = Array.Empty<Contactgegeven>();
    public Vertegenwoordiger[] Vertegenwoordigers { get; set; } = Array.Empty<Vertegenwoordiger>();
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<HoofdactiviteitVerenigingsloket>();
    public Sleutel[] Sleutels { get; set; } = Array.Empty<Sleutel>();
    public Metadata Metadata { get; set; } = null!;


    public record VerenigingsType
    {
        public string Code { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
    }

    public record Contactgegeven
    {
        public int ContactgegevenId { get; set; }
        public string Type { get; set; } = null!;
        public string Waarde { get; set; } = null!;
        public string? Beschrijving { get; set; }
        public bool IsPrimair { get; set; }
    }

    public record Locatie
    {
        public string Locatietype { get; set; } = null!;

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

    public record Vertegenwoordiger
    {
        public int VertegenwoordigerId { get; set; }
        public string Voornaam { get; set; } = null!;
        public string Achternaam { get; set; } = null!;
        public string? Roepnaam { get; set; }
        public string? Rol { get; set; }
        public bool IsPrimair { get; set; }
        public string Email { get; set; } = null!;
        public string Telefoon { get; set; } = null!;
        public string Mobiel { get; set; } = null!;
        public string SocialMedia { get; set; } = null!;
    }

    public record HoofdactiviteitVerenigingsloket
    {
        public string Code { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
    }

    public record Sleutel
    {
        public string Bron { get; set; } = null!;
        public string Waarde { get; set; } = null!;
    }
}
