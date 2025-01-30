namespace AssociationRegistry.Kbo;

using Vereniging;

public record VerenigingVolgensKbo
{
    public KboNummer KboNummer { get; init; } = null!;
    public Verenigingstype Type { get; set; } = null!;
    public string? Naam { get; set; }
    public string? KorteNaam { get; set; }
    public DateOnly? Startdatum { get; set; }
    public AdresVolgensKbo Adres { get; set; } = null!;
    public ContactgegevensVolgensKbo Contactgegevens { get; set; } = null!;
    public VertegenwoordigerVolgensKbo[] Vertegenwoordigers { get; set; } = Array.Empty<VertegenwoordigerVolgensKbo>();
    public DateOnly? EindDatum { get; set; }
    public bool IsActief { get; set; }
}
