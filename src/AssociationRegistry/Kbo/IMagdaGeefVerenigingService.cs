namespace AssociationRegistry.Kbo;

using Framework;
using ResultNet;
using Vereniging;

public interface IMagdaGeefVerenigingService
{
    Task<Result<VerenigingVolgensKbo>> GeefVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken);
    Task<Result<VerenigingVolgensKbo>> GeefSyncVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken);
}
