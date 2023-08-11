namespace AssociationRegistry.Kbo;

using Magda;
using ResultNet;
using Vereniging;

public class VerenigingVolgensKbo
{
    public KboNummer KboNummer { get; init; } = null!;
    public Rechtsvorm Rechtsvorm { get; set; } = null!;
    public string? Naam { get; set; }
    public string? KorteNaam { get; set; }
    public DateOnly? StartDatum { get; set; }
    public AdresVolgensKbo? Adres { get; set; } = null!;
}

public class AdresVolgensKbo
{
    public string? Straatnaam { get; set; }
    public string? Huisnummer { get; set; }
    public string? Busnummer { get; set; }
    public string? Postcode { get; set; }
    public string? Gemeente { get; set; }
    public string? Land { get; set; }
}
