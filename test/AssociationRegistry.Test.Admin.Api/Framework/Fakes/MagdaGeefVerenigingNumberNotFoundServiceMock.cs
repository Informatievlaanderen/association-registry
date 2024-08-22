namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.Framework;
using Kbo;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingNumberNotFoundServiceMock : IMagdaGeefVerenigingService
{
    public Task<Result<VerenigingVolgensKbo>> GeefVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
        => Task.FromResult(VerenigingVolgensKboResult.GeenGeldigeVereniging);
}
