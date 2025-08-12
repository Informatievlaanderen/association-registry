namespace AssociationRegistry.Magda.Kbo;

public class AdresVolgensKbo
{
    public string? Straatnaam { get; set; }
    public string? Huisnummer { get; set; }
    public string? Busnummer { get; set; }
    public string? Postcode { get; set; }
    public string? Gemeente { get; set; }
    public string? Land { get; set; }

    public bool IsEmpty()
        =>
            Straatnaam is null &&
            Huisnummer is null &&
            Busnummer is null &&
            Postcode is null &&
            Gemeente is null &&
            Land is null;
}
