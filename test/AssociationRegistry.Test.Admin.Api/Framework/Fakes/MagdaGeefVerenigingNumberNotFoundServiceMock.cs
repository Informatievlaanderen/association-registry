namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.Framework;
using Kbo;
using ResultNet;
using System.Threading;
using System.Threading.Tasks;
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
