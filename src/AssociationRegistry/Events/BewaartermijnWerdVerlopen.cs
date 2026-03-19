namespace AssociationRegistry.Events;

using NodaTime;

public record BewaartermijnWerdVerlopen(
    string BewaartermijnId,
    string VCode,
    string BewaartermijnType,
    int EntityId
) : IEvent;
