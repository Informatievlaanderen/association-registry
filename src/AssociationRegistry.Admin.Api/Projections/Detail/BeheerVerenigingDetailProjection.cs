namespace AssociationRegistry.Admin.Api.Projections.Detail;

using System;
using System.Linq;
using Events;
using Infrastructure.Extensions;
using Framework;
using Vereniging;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;

public record Metadata(long Sequence);

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
            DatumLaatsteAanpassing = verenigingWerdGeregistreerd.Data.DatumLaatsteAanpassing ?? DateOnly.Parse(verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Tijdstip)),
            Status = "Actief",
            ContactInfoLijst = verenigingWerdGeregistreerd.Data.ContactInfoLijst?.Select(
                            c => new BeheerVerenigingDetailDocument.ContactInfo()
                            {
                                Contactnaam = c.Contactnaam,
                                Email = c.Email,
                                Telefoon = c.Telefoon,
                                Website = c.Website,
                                SocialMedia = c.SocialMedia,
                            }).ToArray()
                        ?? Array.Empty<BeheerVerenigingDetailDocument.ContactInfo>(),
            Locaties = verenigingWerdGeregistreerd.Data.Locaties?.Select(MapLocatie).ToArray() ?? Array.Empty<BeheerVerenigingDetailDocument.Locatie>(),
            Metadata = new Metadata(verenigingWerdGeregistreerd.Sequence),
        };

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

public class BeheerVerenigingDetailDocument : IVCode, IMetadata
{
    [Identity] public string VCode { get; init; } = null!;

    public string Naam { get; set; } = null!;
    public string? KorteNaam { get; set; }
    public string? KorteBeschrijving { get; set; }
    public DateOnly? Startdatum { get; set; }
    public string? KboNummer { get; set; }
    public string Status { get; set; } = null!;
    public DateOnly DatumLaatsteAanpassing { get; set; }
    public Locatie[] Locaties { get; set; } = null!;
    public ContactInfo[] ContactInfoLijst { get; set; } = Array.Empty<ContactInfo>();
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
}
