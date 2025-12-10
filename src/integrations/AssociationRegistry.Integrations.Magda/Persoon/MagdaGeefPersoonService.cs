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
    private readonly IMagdaClient _magdaClient;

    public MagdaGeefPersoonService(IMagdaClient magdaClient, IMagdaRegistreerInschrijvingValidator magdaRegistreerInschrijvingValidator, IMagdaGeefPersoonValidator magdaGeefPersoonValidator, ILogger<MagdaGeefPersoonService> logger)
    {
        _magdaClient = magdaClient;
        _magdaRegistreerInschrijvingValidator = magdaRegistreerInschrijvingValidator;
        _magdaGeefPersoonValidator = magdaGeefPersoonValidator;
        _logger = logger;
    }

    public async Task<PersonenUitKsz> GeefPersonen(GeefPersoonRequest[] vertegenwoordigers, CommandMetadata metadata, CancellationToken cancellationToken)
    {
        var tasks = vertegenwoordigers
                   .Select(v => GeefPersoon(v, metadata, cancellationToken))
                   .ToArray();

        return new PersonenUitKsz(await Task.WhenAll(tasks));
    }

    public async Task<PersoonUitKsz> GeefPersoon(GeefPersoonRequest vertegenwoordiger, CommandMetadata metadata, CancellationToken cancellationToken)
    {
        await _magdaClient.RegistreerInschrijvingPersoon(
            vertegenwoordiger.Insz,
            AanroependeFunctie.RegistreerVzer,
            metadata,
            cancellationToken);

        var persoon = await _magdaClient.GeefPersoon(
            vertegenwoordiger.Insz,
            AanroependeFunctie.RegistreerVzer,
            metadata,
            cancellationToken);

        return new PersoonUitKsz(
            vertegenwoordiger.Insz,
            vertegenwoordiger.Voornaam,
            vertegenwoordiger.Achternaam,
            persoon.Body.GeefPersoonResponse.Repliek.Antwoorden.Antwoord.Inhoud.Persoon.Overlijden != null);
    }
}
