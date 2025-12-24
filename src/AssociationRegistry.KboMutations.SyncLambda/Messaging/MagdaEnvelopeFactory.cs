namespace AssociationRegistry.KboMutations.SyncLambda.Messaging;

using System.Diagnostics;

internal static class MagdaEnvelopeFactory
{
    public static MagdaEnvelope Create(
        string? kbo,
        string? insz,
        bool? overleden,
        ActivityContext? parentContext,
        string? sourceFileName)
    {
        if (!string.IsNullOrWhiteSpace(kbo) && string.IsNullOrWhiteSpace(insz))
        {
            return new MagdaEnvelope(
                MagdaMessageType.SyncKbo,
                kbo,
                null,
                parentContext,
                sourceFileName
            );
        }

        if (!string.IsNullOrWhiteSpace(insz) && string.IsNullOrWhiteSpace(kbo))
        {
            return new MagdaEnvelope(
                MagdaMessageType.SyncKsz,
                null,
                new TeSynchroniserenInszMessage(insz, overleden ?? false),
                parentContext,
                sourceFileName
            );
        }

        return new MagdaEnvelope(
            MagdaMessageType.Unknown,
            kbo,
            insz is null ? null : new TeSynchroniserenInszMessage(insz, overleden ?? false),
            parentContext,
            sourceFileName
        );
    }
}
