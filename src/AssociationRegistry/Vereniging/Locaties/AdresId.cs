namespace AssociationRegistry.Vereniging;

public record AdresId
{
    public Adresbron Adresbron { get; }
    public string Bronwaarde { get; }

    private AdresId(Adresbron adresbron, string bronwaarde)
    {
        Adresbron = adresbron;
        Bronwaarde = bronwaarde;
    }

    public static AdresId Create(Adresbron adresbron, string waarde)
        => new(adresbron, waarde);
}
