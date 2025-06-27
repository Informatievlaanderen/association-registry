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
        Task<Result<PotentialDuplicatesFound>>
        BeforeAsync(
            CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
            EnrichedLocaties enrichedLocaties,
            IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
            ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> logger,
            CancellationToken cancellation)
    {
        if (!envelope.Command.SkipDuplicateDetection)
        {
            var duplicates = (await duplicateVerenigingDetectionService.ExecuteAsync(envelope.Command.Naam, envelope.Command.Locaties))
               .ToList();

            if (duplicates.Any())
            {
                return
                    new Result<PotentialDuplicatesFound>(new PotentialDuplicatesFound(duplicates), ResultStatus.Failed);
            }
        }

        return new Result<PotentialDuplicatesFound>(new PotentialDuplicatesFound([]), ResultStatus.Succeed);
    }
}

public class DuplicateCheckResult
{
}
