namespace AssociationRegistry.Admin.Schema.Bewaartermijn;

using Marten.Schema;
using NodaTime;

public record BewaartermijnDocument(
    [property: Identity] string BewaartermijnId,
    string VCode,
    string BewaartermijnType,
    int RecordId,
    Instant Vervaldag,
    string Reden
);
