namespace AssociationRegistry.Admin.Api.Projections.Detail;

using System;
using System.Linq;
using Events;
using Events.CommonEventDataTypes;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;
using IEvent = Marten.Events.IEvent;

public record Metadata(long Sequence, long Version);

public class BeheerVerenigingDetailProjection : SingleStreamAggregation<BeheerVerenigingDetailDocument>
{
    public BeheerVerenigingDetailDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
        => new()
        {
            VCode = verenigingWerdGeregistreerd.Data.VCode,
            Naam = verenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = verenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = verenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = verenigingWerdGeregistreerd.Data.Startdatum,
            KboNummer = verenigingWerdGeregistreerd.Data.KboNummer,
            DatumLaatsteAanpassing = verenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = "Actief",
            ContactInfoLijst = verenigingWerdGeregistreerd.Data.ContactInfoLijst.Select(
                c => new BeheerVerenigingDetailDocument.ContactInfo()
                {
                    Contactnaam = c.Contactnaam,
                    Email = c.Email,
                    Telefoon = c.Telefoon,
                    Website = c.Website,
                    SocialMedia = c.SocialMedia,
                    PrimairContactInfo = c.PrimairContactInfo,
                }).ToArray(),
            Locaties = ToLocationArray(verenigingWerdGeregistreerd.Data.Locaties),
            Vertegenwoordigers = verenigingWerdGeregistreerd.Data.Vertegenwoordigers.Select(
                v => new BeheerVerenigingDetailDocument.Vertegenwoordiger
                {
                    PrimairContactpersoon = v.PrimairContactpersoon,
                    Roepnaam = v.Roepnaam,
                    Insz = v.Insz,
                    Rol = v.Rol,
                    Achternaam = v.Achternaam,
                    Voornaam = v.Voornaam,
                    ContactInfoLijst = v.ContactInfoLijst.Select(ContactInfoFromEvent).ToArray(),
                }).ToArray(),
            HoofdactiviteitenVerenigingsloket = verenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
                h => new BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket()
                {
                    Code = h.Code,
                    Beschrijving = h.Beschrijving,
                }).ToArray(),
            Metadata = new Metadata(verenigingWerdGeregistreerd.Sequence, verenigingWerdGeregistreerd.Version),
        };

    private static BeheerVerenigingDetailDocument.ContactInfo ContactInfoFromEvent(ContactInfo c)
        => new()
        {
            Contactnaam = c.Contactnaam,
            Email = c.Email,
            Telefoon = c.Telefoon,
            Website = c.Website,
            SocialMedia = c.SocialMedia,
            PrimairContactInfo = c.PrimairContactInfo,
        };

    private static BeheerVerenigingDetailDocument.Locatie[] ToLocationArray(VerenigingWerdGeregistreerd.Locatie[]? locaties)
        => locaties?.Select(MapLocatie).ToArray() ?? Array.Empty<BeheerVerenigingDetailDocument.Locatie>();

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
        document.DatumLaatsteAanpassing = naamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = document.Metadata with { Sequence = naamWerdGewijzigd.Sequence, Version = naamWerdGewijzigd.Version };
    }

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;
        document.DatumLaatsteAanpassing = korteNaamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = document.Metadata with { Sequence = korteNaamWerdGewijzigd.Sequence, Version = korteNaamWerdGewijzigd.Version };
    }

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
        document.DatumLaatsteAanpassing = korteBeschrijvingWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = document.Metadata with { Sequence = korteBeschrijvingWerdGewijzigd.Sequence, Version = korteBeschrijvingWerdGewijzigd.Version };
    }

    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Startdatum = startdatumWerdGewijzigd.Data.Startdatum;
        document.DatumLaatsteAanpassing = startdatumWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = document.Metadata with { Sequence = startdatumWerdGewijzigd.Sequence, Version = startdatumWerdGewijzigd.Version };
    }

    public void Apply(IEvent<ContactInfoLijstWerdGewijzigd> contactInfoLijstWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.ContactInfoLijst = document.ContactInfoLijst
            .Concat(contactInfoLijstWerdGewijzigd.Data.Toevoegingen.Select(ContactInfoFromEvent))
            .Except(contactInfoLijstWerdGewijzigd.Data.Verwijderingen.Select(ContactInfoFromEvent))
            .ExceptBy(contactInfoLijstWerdGewijzigd.Data.Wijzigingen.Select(info => info.Contactnaam), info => info.Contactnaam)
            .Concat(contactInfoLijstWerdGewijzigd.Data.Wijzigingen.Select(ContactInfoFromEvent))
            .ToArray();
        document.DatumLaatsteAanpassing = contactInfoLijstWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
        document.Metadata = document.Metadata with { Sequence = contactInfoLijstWerdGewijzigd.Sequence, Version = contactInfoLijstWerdGewijzigd.Version };
    }

    private static BeheerVerenigingDetailDocument.Locatie MapLocatie(VerenigingWerdGeregistreerd.Locatie loc)
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
    public string? KorteNaam { get; set; }
    public string? KorteBeschrijving { get; set; }
    public DateOnly? Startdatum { get; set; }
    public string? KboNummer { get; set; }
    public string Status { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = Array.Empty<Locatie>();
    public ContactInfo[] ContactInfoLijst { get; set; } = Array.Empty<ContactInfo>();
    public Vertegenwoordiger[] Vertegenwoordigers { get; set; } = Array.Empty<Vertegenwoordiger>();
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<HoofdactiviteitVerenigingsloket>();
    public Metadata Metadata { get; set; } = null!;

    public record ContactInfo
    {
        public string Contactnaam { get; set; } = null!;
        public string? Email { get; set; }
        public string? Telefoon { get; set; }
        public string? Website { get; set; }
        public string? SocialMedia { get; set; }
        public bool PrimairContactInfo { get; set; }
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
        public string Insz { get; set; } = null!;
        public string Voornaam { get; set; } = null!;
        public string Achternaam { get; set; } = null!;
        public string? Roepnaam { get; set; }
        public string? Rol { get; set; }
        public bool PrimairContactpersoon { get; set; }
        public ContactInfo[] ContactInfoLijst { get; set; } = Array.Empty<ContactInfo>();
    }

    public record HoofdactiviteitVerenigingsloket
    {
        public string Code { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
    }
}
