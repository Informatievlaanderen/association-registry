namespace AssociationRegistry.Admin.Api.GrarConsumer.Handlers.Fusies;

public interface IAdresMergerHandler
{
    Task Handle(int sourceAdresId, int? destinationAdresId);
}

public class AdresMergerHandler : IAdresMergerHandler
{
    private readonly ITeHeradresserenLocatiesHandler _teHeradresserenLocatiesHandler;
    private readonly ITeOntkoppelenLocatiesHandler _teOntkoppelenLocatiesHandler;

    public AdresMergerHandler(
        ITeHeradresserenLocatiesHandler teHeradresserenLocatiesHandler,
        ITeOntkoppelenLocatiesHandler teOntkoppelenLocatiesHandler)
    {
        _teHeradresserenLocatiesHandler = teHeradresserenLocatiesHandler;
        _teOntkoppelenLocatiesHandler = teOntkoppelenLocatiesHandler;
    }

    public async Task Handle(int sourceAdresId, int? destinationAdresId)
    {
        if (!destinationAdresId.HasValue)
            await _teOntkoppelenLocatiesHandler.Handle(sourceAdresId);
        else
            await _teHeradresserenLocatiesHandler.Handle(sourceAdresId, destinationAdresId.Value);
    }
}

public interface ITeHeradresserenLocatiesHandler
{
    Task Handle(int sourceAdresId, int destinationAdresId);
}

public interface ITeOntkoppelenLocatiesHandler
{
    Task Handle(int sourceAdresId);
}
