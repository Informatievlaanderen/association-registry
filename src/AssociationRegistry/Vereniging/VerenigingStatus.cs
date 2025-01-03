namespace AssociationRegistry.Vereniging;

public abstract record VerenigingStatus(string StatusNaam)
{
    public record StatusActief() : VerenigingStatus("Actief");
    public record StatusGestopt() : VerenigingStatus("Gestopt");
    public record StatusDubbel(VCode VCodeAuthentiekeVereniging, VerenigingStatus VorigeVerenigingStatus) : VerenigingStatus("Dubbel");

    public static VerenigingStatus Actief => new StatusActief();
    public static VerenigingStatus Gestopt => new StatusGestopt();

    public VerenigingStatus ParseVorigeStatus(string vorigeStatus)
    {
        switch (vorigeStatus)
        {
            case "Actief": return new StatusActief();
            case "Gestopt": return new StatusGestopt();
            default: throw new ArgumentOutOfRangeException();
        }
    }
}

