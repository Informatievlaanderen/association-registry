namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.Framework;
using Kbo;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingNumberFoundServiceMock : IMagdaGeefVerenigingService
{
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;

    public MagdaGeefVerenigingNumberFoundServiceMock(VerenigingVolgensKbo verenigingVolgensKbo)
    {
        _verenigingVolgensKbo = verenigingVolgensKbo;
    }

    public Task<Result<VerenigingVolgensKbo>> GeefVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
        => Task.FromResult(VerenigingVolgensKboResult.GeldigeVereniging(_verenigingVolgensKbo));
}
