namespace AssociationRegistry.Integrations.Magda.Persoon;

using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Magda.Persoon;
using DecentraalBeheer.Vereniging;
using Framework;
using Microsoft.Extensions.Logging;
using Validation;

public class MagdaGeefPersoonService : IMagdaGeefPersoonService
{
    private readonly IMagdaRegistreerInschrijvingValidator _magdaRegistreerInschrijvingValidator;
    private readonly IMagdaGeefPersoonValidator _magdaGeefPersoonValidator;
    private readonly ILogger<MagdaGeefPersoonService> _logger;
    private IMagdaClient MagdaClient { get; }

    public MagdaGeefPersoonService(IMagdaClient magdaClient, IMagdaRegistreerInschrijvingValidator magdaRegistreerInschrijvingValidator, IMagdaGeefPersoonValidator magdaGeefPersoonValidator, ILogger<MagdaGeefPersoonService> logger)
    {
        MagdaClient = magdaClient;
        _magdaRegistreerInschrijvingValidator = magdaRegistreerInschrijvingValidator;
        _magdaGeefPersoonValidator = magdaGeefPersoonValidator;
        _logger = logger;
    }

    public async Task<PersonenUitKsz> GeefPersonen(Vertegenwoordiger[] vertegenwoordigers, CommandMetadata metadata, CancellationToken cancellationToken)
    {
        var tasks = vertegenwoordigers
                   .Select(v => GeefPersoon(v, metadata, cancellationToken))
                   .ToArray();

        return new PersonenUitKsz(await Task.WhenAll(tasks));
    }

    public async Task<PersoonUitKsz> GeefPersoon(Vertegenwoordiger vertegenwoordiger, CommandMetadata metadata, CancellationToken cancellationToken)
    {
        // First: Register subscription (must succeed)
        var registreerInschrijvingResponse = await MagdaClient.RegistreerInschrijvingPersoon(
            vertegenwoordiger.Insz,
            AanroependeFunctie.RegistreerVzer,
            metadata,
            cancellationToken);

            _magdaRegistreerInschrijvingValidator.ValidateOrThrow(registreerInschrijvingResponse);

        // Second: Get person details (only runs if registration succeeded)
        var persoon = await MagdaClient.GeefPersoon(
            vertegenwoordiger.Insz,
            AanroependeFunctie.RegistreerVzer,
            metadata,
            cancellationToken);

            _magdaGeefPersoonValidator.ValidateOrThrow(persoon);

        return new PersoonUitKsz(
            vertegenwoordiger.Insz,
            vertegenwoordiger.Voornaam,
            vertegenwoordiger.Achternaam,
            persoon.Body.GeefPersoonResponse.Repliek.Antwoorden.Antwoord.Inhoud.Persoon.Overlijden != null);
    }
}
