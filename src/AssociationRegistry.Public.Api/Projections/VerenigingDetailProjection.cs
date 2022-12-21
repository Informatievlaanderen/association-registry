namespace AssociationRegistry.Public.Api.Projections;

using System;
using System.Linq;
using Framework;
using Infrastructure;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;
using Vereniging;

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

    public ContactInfo[] Contacten { get; set; } = Array.Empty<ContactInfo>();

    public class ContactInfo
    {
        public string? Contactnaam { get; set; }
        public string? Email { get; set; }
        public string? Telefoon { get; set; }
        public string? Website { get; set; }
        public string? SocialMedia { get; set; }
    }
}
