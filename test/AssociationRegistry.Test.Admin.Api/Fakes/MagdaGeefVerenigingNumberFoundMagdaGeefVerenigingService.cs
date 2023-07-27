namespace AssociationRegistry.Test.Admin.Api.Fakes;

using Kbo;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingNumberFoundMagdaGeefVerenigingService:IMagdaGeefVerenigingService
{
    public Task<Result> GeefVereniging(string kboNummer)
        => Task.FromResult<Result>(VerenigingVolgensKboResult.GeldigeVereniging(new VerenigingVolgensKbo { KboNummer = KboNummer.Create(kboNummer)}));
}
