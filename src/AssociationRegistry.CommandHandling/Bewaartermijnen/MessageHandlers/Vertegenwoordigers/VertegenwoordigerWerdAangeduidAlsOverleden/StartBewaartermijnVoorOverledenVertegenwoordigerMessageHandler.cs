namespace AssociationRegistry.CommandHandling.Bewaartermijnen.MessageHandlers.Vertegenwoordigers.
    VertegenwoordigerWerdAangeduidAlsOverleden;

using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using Framework;
using Integrations.Grar.Bewaartermijnen;
using MartenDb.Store;

public class StartBewaartermijnVoorOverledenVertegenwoordigerMessageHandler
{
    public static async Task Handle(
        StartBewaartermijnVoorOverledenVertegenwoordigerMessage message,
        IEventStore eventStore,
        BewaartermijnOptions bewaartermijnOptions,
        CancellationToken cancellationToken
    )
    {
        await CreateBewaartermijn(
            @event.StreamKey!,
            @event.Data.VertegenwoordigerId,
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip),
            eventStore,
            bewaartermijnOptions,
            cancellationToken,
            BewaartermijnReden.KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden
        );
    }
}
