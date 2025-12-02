namespace AssociationRegistry.Test.Admin.Api.Framework.Fakes;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using Integrations.Magda;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingNumberNotFoundServiceMock : IMagdaGeefVerenigingService
{
    public async Task<Result> GeefVereniging(
        KboNummer kboNummer,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
        => VerenigingVolgensKboResult.GeenGeldigeVereniging;
}
