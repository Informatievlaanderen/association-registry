namespace AssociationRegistry.Events;

public record BewaartermijnWerdGestart(string BewaartermijnId, string VCode, int VertegenwoordigerId) : IEvent;
