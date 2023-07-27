namespace AssociationRegistry.Kbo;

using ResultNet;

public class VerenigingVolgensKboResult
{
    public static Result<VerenigingVolgensKbo> GeldigeVereniging(VerenigingVolgensKbo verenigingVolgensKbo)
        => Result.Success(verenigingVolgensKbo);

    public static Result<GeefVereniging.NietGevonden> GeenGeldigeVereniging
        => new(new GeefVereniging.NietGevonden(), ResultStatus.Failed);

    public static Result<GeefVereniging.OnbekendeFout> OnbekendeFout(Exception e)
        => Result.Failure<GeefVereniging.OnbekendeFout>().WithError(e);
}
