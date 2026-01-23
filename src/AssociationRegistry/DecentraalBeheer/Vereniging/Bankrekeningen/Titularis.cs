namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;

using Exceptions;

public record Titularis
{
    public string Value { get; }

    private Titularis(string value)
    {
        Value = value;
    }

    public static Titularis Create(string titularis)
    {
        if (string.IsNullOrEmpty(titularis))
            throw new TitularisMagNietNullOfLeegZijn();

        return new Titularis(titularis);
    }

    public static Titularis Hydrate(string titularis)
        => new(titularis);

    public Titularis Replace(string? titularis)
        => titularis == null ? this : Create(titularis);
}
