﻿namespace AssociationRegistry.Magda;

using Vereniging;

public class StaticMagdaFacade : IMagdaFacade
{
    public Task<MagdaPersoon> GetByInsz(Insz insz, CancellationToken cancellationToken = default)
        => Task.FromResult(
            new MagdaPersoon
            {
                Insz = insz,
                Voornaam = "Jhon",
                Achternaam = "Doo",
                IsOverleden = true,
            });
}
