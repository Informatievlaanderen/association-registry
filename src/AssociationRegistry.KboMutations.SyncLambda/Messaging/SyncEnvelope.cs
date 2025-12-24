namespace AssociationRegistry.KboMutations.SyncLambda.Messaging;

using System.Diagnostics;

public record SyncEnvelope(
    SyncMessageType Type,
    string? KboNummer,
    TeSynchroniserenInszMessage? InszMessage,
    ActivityContext? ParentTraceContext,
    string? SourceFileName,
    Guid CorrelationId
);
