namespace AssociationRegistry.Middleware;

using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using DuplicateVerenigingDetection;
using Framework;
using Microsoft.Extensions.Logging;
using ResultNet;
using Wolverine;

public class DuplicateDetectionMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async
        Task<PotentialDuplicatesFound>
        BeforeAsync(
            CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
            VerrijkteAdressenUitGrar verrijkteAdressenUitGrar,
            IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
            ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> logger,
            CancellationToken cancellation)
    {
        var registrationHasNoLocations = envelope.Command.Locaties.Length == 0 && verrijkteAdressenUitGrar.Count == 0;

        if (envelope.Command.SkipDuplicateDetection || registrationHasNoLocations)
            return PotentialDuplicatesFound.None;

        var locaties = new DuplicateVerenigingZoekQueryLocaties(envelope.Command.Locaties)
           .VerrijkMetVerrijkteAdressenUitGrar(verrijkteAdressenUitGrar);

        var duplicates = (await duplicateVerenigingDetectionService.ExecuteAsync(envelope.Command.Naam, locaties))
           .ToArray();

        if (duplicates.Any())
            return PotentialDuplicatesFound.Some(duplicates);

        return PotentialDuplicatesFound.None;
    }
}
