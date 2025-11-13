namespace AssociationRegistry.Events;

public record VertegenwoordigerWerdVerwijderd(Guid RefId, int VertegenwoordigerId) : IEvent
{
}
