namespace AssociationRegistry.Magda.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;

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
    public BankrekeningnummerVolgensKbo[] Bankrekeningnummers { get; set; } = Array.Empty<BankrekeningnummerVolgensKbo>();
    public DateOnly? EindDatum { get; set; }
    public bool IsActief { get; set; }
}

public record InactieveVereniging
{
    public KboNummer KboNummer { get; init; } = null!;
    public DateOnly? EindDatum { get; set; }
}

