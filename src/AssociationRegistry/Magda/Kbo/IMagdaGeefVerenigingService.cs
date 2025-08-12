namespace AssociationRegistry.Magda.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using ResultNet;

public interface IMagdaGeefVerenigingService
{
    Task<Result> GeefVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<Result> GeefSyncVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken);
}
