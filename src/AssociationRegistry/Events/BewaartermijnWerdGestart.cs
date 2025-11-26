namespace AssociationRegistry.Events;

using NodaTime;

public record BewaartermijnWerdGestart(string BewaartermijnId, string VCode, int VertegenwoordigerId, Instant Vervaldag) : IEvent;
