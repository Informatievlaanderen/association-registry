namespace AssociationRegistry.Kbo;

using ResultNet;

public class VerenigingVolgensKboResult
{
    public static Result GeldigeVereniging(VerenigingVolgensKbo verenigingVolgensKbo)
        => Result.Success(verenigingVolgensKbo);

    public static Result InactieveVereniging(InactieveVereniging verenigingVolgensKbo)
        => Result.Success(verenigingVolgensKbo);

    public static Result<VerenigingVolgensKbo> GeenGeldigeVereniging
        => Result.Failure<VerenigingVolgensKbo>();
}
