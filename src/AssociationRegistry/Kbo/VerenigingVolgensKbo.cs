namespace AssociationRegistry.Kbo;

using Vereniging;

public class VerenigingVolgensKbo
{
    public KboNummer KboNummer { get; init; } = null!;
    public Verenigingstype Type { get; set; } = null!;
    public string? Naam { get; set; }
    public string? KorteNaam { get; set; }
    public DateOnly? StartDatum { get; set; }
    public AdresVolgensKbo? Adres { get; set; } = null!;
    public ContactgegevensVolgensKbo? Contactgegevens { get; set; }
}
