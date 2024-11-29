namespace AssociationRegistry.Admin.Api.GrarConsumer.Handlers;

public interface IAdresMergerHandler
{
    Task Handle(int sourceAdresId, int? destinationAdresId);
}

public class AdresMergerHandler : IAdresMergerHandler
{
    private readonly ITeHeradresserenLocatiesHandler _teHeradresserenLocatiesHandler;

    public AdresMergerHandler(ITeHeradresserenLocatiesHandler teHeradresserenLocatiesHandler)
    {
        _teHeradresserenLocatiesHandler = teHeradresserenLocatiesHandler;
    }

    public async Task Handle(int sourceAdresId, int? destinationAdresId)
    {
        // if null -> TeOnkoppelenHandler
        // send teOnkoppelenMessages


        // if not null -> zie code hieronder via TeHeradresserenLocatiesHandler
        if (destinationAdresId.HasValue)
            await _teHeradresserenLocatiesHandler.Handle(sourceAdresId, destinationAdresId.Value);
    }
}

public interface ITeHeradresserenLocatiesHandler
{
    Task Handle(int sourceAdresId, int destinationAdresId);
}
