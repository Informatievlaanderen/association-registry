﻿namespace AssociationRegistry.Grar.GrarUpdates.Fusies;

public interface IFusieEventHandler
{
    Task Handle(int sourceAdresId, int? destinationAdresId);
}

public class FusieEventHandler : IFusieEventHandler
{
    private readonly ITeHeradresserenLocatiesHandler _teHeradresserenLocatiesHandler;
    private readonly ITeOntkoppelenLocatiesHandler _teOntkoppelenLocatiesHandler;

    public FusieEventHandler(
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
