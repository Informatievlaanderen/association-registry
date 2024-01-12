namespace AssociationRegistry.Events;

using Framework;

public record VerenigingWerdVerwijderd(string Reden) : IEvent
{
    public static VerenigingWerdVerwijderd With(string reden)
        => new(reden);
}
