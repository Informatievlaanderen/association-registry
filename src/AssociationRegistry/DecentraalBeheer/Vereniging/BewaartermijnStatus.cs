namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public abstract record BewaartermijnStatus(string StatusNaam)
{
    public record StatusGepland() : BewaartermijnStatus(Naam)
    {
        public const string Naam = "Gepland";
    }

    public record StatusVerlopen() : BewaartermijnStatus(Naam)
    {
        public const string Naam = "Verlopen";
    }

    public static BewaartermijnStatus Gepland => new StatusGepland();
    public static BewaartermijnStatus Verlopen => new StatusVerlopen();
}

