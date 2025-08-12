namespace AssociationRegistry.DecentraalBeheer.Vereniging.Adressen.GemeentenaamVerrijking;

using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Grar.Models.PostalInfo;

public record VerrijkteGemeentenaam: Gemeentenaam
{
    public Postnaam? Postnaam { get; }
    public string Gemeentenaam { get; }

    private VerrijkteGemeentenaam(Postnaam? postnaam, string gemeentenaam)
        : base(Format(postnaam, gemeentenaam))
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
        if (string.IsNullOrEmpty(postnaam.Value))
            throw new ArgumentException(nameof(postnaam));

        if (string.IsNullOrEmpty(gemeentenaam))
            throw new ArgumentException(nameof(gemeentenaam));

        return new(postnaam, gemeentenaam);
    }

    public string Format()
        => Format(Postnaam, Gemeentenaam);

    private static string Format(Postnaam? postnaam, string gemeentenaam)
    {
        if (postnaam is not null)
            return $"{postnaam.Value} ({gemeentenaam})";

        return gemeentenaam;
    }

    public static VerrijkteGemeentenaam FromGemeentenaam(Gemeentenaam gemeentenaam)
    {
        try
        {
            var splittedGemeentenaam = gemeentenaam.Naam.Split('(').Select(x => x.Trim().Trim(')').Trim()).ToArray();

            return splittedGemeentenaam.Count() switch
            {
                1 => ZonderPostnaam(splittedGemeentenaam.First()),
                2 => MetPostnaam(Postnaam.FromValue(splittedGemeentenaam[0]), splittedGemeentenaam[1]),
                _ => ZonderPostnaam(gemeentenaam.Naam),
            };
        }
        catch (Exception e)
        {
            return ZonderPostnaam(gemeentenaam.Naam);
        }
    }
}
