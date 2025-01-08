namespace AssociationRegistry.Events;

using Grar.Models.PostalInfo;

public record VerrijkteGemeentenaam
{
    public Postnaam? Postnaam { get; }
    public string Gemeentenaam { get; }

    private VerrijkteGemeentenaam(Postnaam? postnaam, string gemeentenaam)
    {
        Postnaam = postnaam;
        Gemeentenaam = gemeentenaam;
    }

    public static VerrijkteGemeentenaam ZonderPostnaam(string gemeentenaam)
    {
        if (string.IsNullOrEmpty(gemeentenaam))
            throw new ArgumentException(nameof(gemeentenaam));

        return new VerrijkteGemeentenaam(null, gemeentenaam);
    }

    public static VerrijkteGemeentenaam MetPostnaam(Postnaam postnaam, string gemeentenaam)
    {
        if (string.IsNullOrEmpty(postnaam))
            throw new ArgumentException(nameof(postnaam));
        if (string.IsNullOrEmpty(gemeentenaam))
            throw new ArgumentException(nameof(gemeentenaam));

        return new(postnaam, gemeentenaam);
    }

    public string Format()
    {
        if(Postnaam is not null)
            return $"{Postnaam.Value} ({Gemeentenaam})";

        return Gemeentenaam;
    }
}
