namespace AssociationRegistry.Events;

using NodaTime;

public record BewaartermijnWerdVerlopen(string BewaartermijnId, string VCode, int VertegenwoordigerId, Instant Vervaldag) : IEvent;

