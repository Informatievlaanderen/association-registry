namespace AssociationRegistry.Admin.Schema.Search;

using System;

public class VerenigingZoekDocument
{
    public class Locatie
    {
        public string Locatietype { get; init; } = null!;
        public string? Naam { get; init; }
        public string Adres { get; init; } = null!;
        public bool Hoofdlocatie { get; init; }
        public string Postcode { get; init; } = null!;
        public string Gemeente { get; init; } = null!;
    }

    public class HoofdactiviteitVerenigingsloket
    {
        public string Code { get; init; } = null!;
        public string Naam { get; init; } = null!;
    }

    public class VerenigingsType
    {
        public string Code { get; init; } = null!;
        public string Beschrijving { get; init; } = null!;
    }

    public class Sleutel
    {
        public string Bron { get; set; } = null!;
        public string Waarde { get; set; } = null!;
    }

    public string VCode { get; set; } = null!;
    public VerenigingsType Type { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public string KorteNaam { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = null!;
    public string Doelgroep { get; set; } = null!;
    public string[] Activiteiten { get; set; } = null!;
    public Sleutel[] Sleutels { get; set; } = Array.Empty<Sleutel>();

    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }
}
