namespace AssociationRegistry.KboMutations.SyncLambda.Messaging;

using System.Diagnostics;

internal static class SyncEnvelopeFactory
{
    public static SyncEnvelope Create(
        string? kbo,
        string? insz,
        bool? overleden,
        ActivityContext? parentContext,
        string? sourceFileName,
        Guid correlationId)
    {
        if (!string.IsNullOrWhiteSpace(kbo) && string.IsNullOrWhiteSpace(insz))
        {
            return new SyncEnvelope(
                SyncMessageType.SyncKbo,
                kbo,
                null,
                parentContext,
                sourceFileName,
                correlationId
            );
        }

        if (!string.IsNullOrWhiteSpace(insz) && string.IsNullOrWhiteSpace(kbo))
        {
            return new SyncEnvelope(
                SyncMessageType.SyncKsz,
                null,
                new TeSynchroniserenInszMessage(insz, overleden ?? false),
                parentContext,
                sourceFileName,
                correlationId
            );
        }

        return new SyncEnvelope(
            SyncMessageType.Unknown,
            kbo,
            insz is null ? null : new TeSynchroniserenInszMessage(insz, overleden ?? false),
            parentContext,
            sourceFileName,
            correlationId
        );
    }
}
