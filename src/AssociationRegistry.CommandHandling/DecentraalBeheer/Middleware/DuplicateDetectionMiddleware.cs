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
        if (envelope.Command.HeeftBevestigingstoken)
            return HandleWithBevestigingstoken(envelope, bevestigingsTokenHelper);

        if (RegistrationHasNoLocations(envelope, verrijkteAdressenUitGrar))
            return PotentialDuplicatesFound.None;

        var duplicates = await FindDuplicateVerenigingen(envelope, verrijkteAdressenUitGrar, duplicateVerenigingDetectionService);

        if (duplicates.Any())
        {
            var newBevestigingstoken = await RapporteerDuplicateVerenigingen(envelope, rapporteerDubbeleVerenigingenService, bevestigingsTokenHelper, cancellation, duplicates);

            return PotentialDuplicatesFound.Some(newBevestigingstoken, duplicates);
        }

        return PotentialDuplicatesFound.None;
    }

    private static async Task<string> RapporteerDuplicateVerenigingen(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
        IRapporteerDubbeleVerenigingenService rapporteerDubbeleVerenigingenService,
        IBevestigingsTokenHelper bevestigingsTokenHelper,
        CancellationToken cancellation,
        DuplicaatVereniging[] duplicates)
    {
        var newBevestigingstoken = bevestigingsTokenHelper.Calculate(envelope.Command.OriginalRequest);
        await rapporteerDubbeleVerenigingenService.RapporteerAsync(new CommandEnvelope<RapporteerDubbeleVerenigingenMessage>(
                                                                       new RapporteerDubbeleVerenigingenMessage(newBevestigingstoken, envelope.Command.Naam, envelope.Command.Locaties, duplicates),
                                                                       envelope.Metadata), cancellation);

        return newBevestigingstoken;
    }

    private static async Task<DuplicaatVereniging[]> FindDuplicateVerenigingen(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
        VerrijkteAdressenUitGrar verrijkteAdressenUitGrar,
        IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService)
    {
        var locaties = new DuplicateVerenigingZoekQueryLocaties(envelope.Command.Locaties)
           .VerrijkMetVerrijkteAdressenUitGrar(verrijkteAdressenUitGrar);

        var duplicates = (await duplicateVerenigingDetectionService.ExecuteAsync(envelope.Command.Naam, locaties))
           .ToArray();

        return duplicates;
    }

    private static bool RegistrationHasNoLocations(CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope, VerrijkteAdressenUitGrar verrijkteAdressenUitGrar)
        => envelope.Command.Locaties.Length == 0 && verrijkteAdressenUitGrar.Count == 0;

    private static PotentialDuplicatesFound HandleWithBevestigingstoken(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> envelope,
        IBevestigingsTokenHelper bevestigingsTokenHelper)
    {
        var validBevestigingstoken = bevestigingsTokenHelper.IsValid(envelope.Command.Bevestigingstoken, envelope.Command.OriginalRequest);
        Throw<InvalidBevestigingstokenProvided>.If(!validBevestigingstoken);

        return PotentialDuplicatesFound.Skip(envelope.Command.Bevestigingstoken);
    }
}
