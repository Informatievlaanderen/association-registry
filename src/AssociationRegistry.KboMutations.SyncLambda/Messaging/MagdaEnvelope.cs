namespace AssociationRegistry.KboMutations.SyncLambda.Messaging;

using System.Diagnostics;

public record MagdaEnvelope(
    MagdaMessageType Type,
    string? KboNummer,
    TeSynchroniserenInszMessage? InszMessage,
    ActivityContext? ParentTraceContext,
    string? SourceFileName
);
