namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;

using Acties.DubbelDetectie;
using Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Framework;
using Microsoft.Extensions.Logging;

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
            IRapporteerDubbeleVerenigingenService rapporteerDubbeleVerenigingenService,
            IBevestigingsTokenHelper bevestigingsTokenHelper,
            ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> logger,
            CancellationToken cancellation)
    {
        var registrationHasNoLocations = envelope.Command.Locaties.Length == 0 && verrijkteAdressenUitGrar.Count == 0;

        if (envelope.Command.HeeftBevestigingstoken)
        {
            var validBevestigingstoken = bevestigingsTokenHelper.IsValid(envelope.Command.Bevestigingstoken, envelope.Command.OriginalRequest);
            Throw<InvalidBevestigingstokenProvided>.If(!validBevestigingstoken);

            return PotentialDuplicatesFound.Skip(envelope.Command.Bevestigingstoken);
        }

        if (registrationHasNoLocations)
            return PotentialDuplicatesFound.None;

        var locaties = new DuplicateVerenigingZoekQueryLocaties(envelope.Command.Locaties)
           .VerrijkMetVerrijkteAdressenUitGrar(verrijkteAdressenUitGrar);

        var duplicates = (await duplicateVerenigingDetectionService.ExecuteAsync(envelope.Command.Naam, locaties))
           .ToArray();

        if (duplicates.Any())
        {
            var key = Guid.NewGuid().ToString();
            var newBevestigingstoken = bevestigingsTokenHelper.Calculate(envelope.Command.OriginalRequest);
            await rapporteerDubbeleVerenigingenService.RapporteerAsync(new CommandEnvelope<RapporteerDubbeleVerenigingenMessage>(
                                                                           new RapporteerDubbeleVerenigingenMessage(key, newBevestigingstoken, envelope.Command.Naam, envelope.Command.Locaties, duplicates),
                                                                           CommandMetadata.ForDigitaalVlaanderenProcess), cancellation);

            return PotentialDuplicatesFound.Some(newBevestigingstoken, duplicates);
        }

        return PotentialDuplicatesFound.None;
    }
}
