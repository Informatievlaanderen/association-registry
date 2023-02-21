namespace AssociationRegistry.Public.Schema.Detail;

using System;
using Marten.Schema;

public class PubliekVerenigingDetailDocument : IVCode
{
    [Identity] public string VCode { get; set; } = null!;

    public string Naam { get; set; } = null!;
    public string? KorteNaam { get; set; }
    public string? KorteBeschrijving { get; set; }
    public DateOnly? Startdatum { get; set; }
    public string? KboNummer { get; set; }
    public string Status { get; set; } = null!;
    public string DatumLaatsteAanpassing { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public ContactInfo[] ContactInfoLijst { get; set; } = Array.Empty<ContactInfo>();
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = Array.Empty<HoofdactiviteitVerenigingsloket>();

    public class ContactInfo
    {
        public string Contactnaam { get; set; } = null!;
        public string? Email { get; set; }
        public string? Telefoon { get; set; }
        public string? Website { get; set; }
        public string? SocialMedia { get; set; }
        public bool PrimairContactInfo { get; set; }
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

    public class HoofdactiviteitVerenigingsloket
    {
        public string Code { get; set; } = null!;
        public string Beschrijving { get; set; } = null!;
    }
}
