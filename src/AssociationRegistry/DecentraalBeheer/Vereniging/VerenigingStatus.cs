namespace AssociationRegistry.DecentraalBeheer.Vereniging;

public abstract record VerenigingStatus(string StatusNaam)
{
    public record StatusActief() : VerenigingStatus(Naam)
    {
        public const string Naam = "Actief";
    }

    public record StatusGestopt() : VerenigingStatus(Naam)
    {
        public const string Naam = "Gestopt";
    }

    public record StatusDubbel(VCode VCodeAuthentiekeVereniging, VerenigingStatus VorigeVerenigingStatus) : VerenigingStatus("Dubbel");

    public static VerenigingStatus Actief => new StatusActief();
    public static VerenigingStatus Gestopt => new StatusGestopt();

    public static VerenigingStatus ParseVorigeStatus(string vorigeStatus)
    {
        switch (vorigeStatus)
        {
            case StatusActief.Naam: return new StatusActief();
            case StatusGestopt.Naam: return new StatusGestopt();
            default: throw new ArgumentOutOfRangeException();
        }
    }
}

