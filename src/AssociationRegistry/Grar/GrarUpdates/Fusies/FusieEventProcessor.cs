namespace AssociationRegistry.Grar.GrarUpdates.Fusies;

using TeHeradresserenLocaties;
using TeOntkoppelenLocaties;

public class FusieEventProcessor : IFusieEventProcessor
{
    private readonly ITeHeradresserenLocatiesProcessor _teHeradresserenLocatiesHandler;
    private readonly ITeOntkoppelenLocatiesProcessor _teOntkoppelenLocatiesHandler;

    public FusieEventProcessor(
        ITeHeradresserenLocatiesProcessor teHeradresserenLocatiesHandler,
        ITeOntkoppelenLocatiesProcessor teOntkoppelenLocatiesHandler)
    {
        _teHeradresserenLocatiesHandler = teHeradresserenLocatiesHandler;
        _teOntkoppelenLocatiesHandler = teOntkoppelenLocatiesHandler;
    }

    public async Task Process(int sourceAdresId, int? destinationAdresId, string idempotencyKey)
    {
        if (!destinationAdresId.HasValue)
            await _teOntkoppelenLocatiesHandler.Process(sourceAdresId);
        else
            await _teHeradresserenLocatiesHandler.Process(sourceAdresId, destinationAdresId.Value, idempotencyKey);
    }
}
public interface IFusieEventProcessor
{
    Task Process(int sourceAdresId, int? destinationAdresId, string idempotencyKey);
}
