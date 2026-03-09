namespace AssociationRegistry.Events;

using NodaTime;

[Obsolete("This event is not used anymore, use V2 instead")]
public record BewaartermijnWerdGestart(string BewaartermijnId, string VCode, int VertegenwoordigerId, Instant Vervaldag)
    : IEvent;

public record BewaartermijnWerdGestartV2(
    string BewaartermijnId,
    string VCode,
    string BewaartermijnType,
    int RecordId,
    Instant Vervaldag,
    string Reden
) : IEvent;
