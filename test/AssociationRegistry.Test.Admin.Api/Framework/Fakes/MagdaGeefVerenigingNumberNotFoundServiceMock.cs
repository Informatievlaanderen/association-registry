namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingNumberNotFoundServiceMock : IMagdaGeefVerenigingService
{
    public async Task<Result> GeefVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
        => VerenigingVolgensKboResult.GeenGeldigeVereniging;

    public async Task<Result> GeefSyncVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken)
        => VerenigingVolgensKboResult.GeenGeldigeVereniging;
}
