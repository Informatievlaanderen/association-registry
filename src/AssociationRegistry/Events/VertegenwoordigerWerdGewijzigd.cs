namespace AssociationRegistry.Events;

public record VertegenwoordigerWerdGewijzigd(
    Guid RefId,
    int VertegenwoordigerId,
    bool IsPrimair) : IEvent
{ }
