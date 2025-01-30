namespace AssociationRegistry.Kbo;

using Framework;
using ResultNet;
using Vereniging;

public interface IMagdaGeefVerenigingService
{
    Task<Result> GeefVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<Result> GeefSyncVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken);
}
