namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;

using Acties.DubbelDetectie;
using Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.Framework;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
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
            IMessageBus messageBus,
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
        {
            var key = envelope.Command.BevestigingsToken is not null ? envelope.Command.BevestigingsToken.Key : Guid.NewGuid().ToString();
            await messageBus.SendAsync(new CommandEnvelope<RapporteerDubbeleVerenigingenCommand>(
                                           new RapporteerDubbeleVerenigingenCommand(key, envelope.Command.Naam, envelope.Command.Locaties, duplicates),
                                           CommandMetadata.ForDigitaalVlaanderenProcess));
            return PotentialDuplicatesFound.Some(duplicates);
        }

        return PotentialDuplicatesFound.None;
    }
}
