namespace AssociationRegistry.Public.Schema.Search;

public class VerenigingZoekDocument : ICanBeUitgeschrevenUitPubliekeDatastroom, IHasStatus
{
    public string VCode { get; set; } = null!;
    public VerenigingsType Type { get; set; } = null!;
    public string Naam { get; set; } = null!;
    public string? Roepnaam { get; set; }
    public string KorteNaam { get; set; } = null!;
    public Doelgroep Doelgroep { get; set; } = null!;
    public Locatie[] Locaties { get; set; } = null!;
    public HoofdactiviteitVerenigingsloket[] HoofdactiviteitenVerenigingsloket { get; set; } = null!;
    public Sleutel[] Sleutels { get; set; } = null!;
    public Relatie[] Relaties { get; set; } = null!;
    public bool IsUitgeschrevenUitPubliekeDatastroom { get; set; }
    public string Status { get; set; } = null!;

    public class Locatie
    {
        public string Locatietype { get; init; } = null!;
        public string? Naam { get; init; }
        public string Adresvoorstelling { get; init; } = null!;
        public bool IsPrimair { get; init; }
        public string Postcode { get; init; } = null!;
        public string Gemeente { get; init; } = null!;
        public int LocatieId { get; set; }
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
}

public class Doelgroep
{
    public int Minimumleeftijd { get; set; }
    public int Maximumleeftijd { get; set; }
}

public class Relatie
{
    public string Type { get; set; } = null!;
    public GerelateerdeVereniging AndereVereniging { get; set; } = null!;
}

public class GerelateerdeVereniging
{
    public string KboNummer { get; set; } = null!;
    public string VCode { get; set; } = null!;
    public string Naam { get; set; } = null!;
}
