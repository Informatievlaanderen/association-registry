namespace AssociationRegistry.Events;



public record VerenigingWerdVerwijderd(string VCode, string Reden) : IEvent
{
    public static VerenigingWerdVerwijderd With(string vCode, string reden)
        => new(vCode, reden);
}
