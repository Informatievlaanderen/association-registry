namespace AssociationRegistry.Integrations.Magda.Persoon;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Framework;
using Magda;
using Repertorium.RegistreerInschrijving0200;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Magda.Persoon;
using Microsoft.Extensions.Logging;
using Models.RegistreerInschrijving0200;
using Shared.Exceptions;
using Shared.Models;

public record RegistreerInschrijvingPersoonUitzonderingType(string Identificatie, string Beschrijving)
{
    public static readonly RegistreerInschrijvingPersoonUitzonderingType Fout30002 = new("30002", "Het INSZ is geannuleerd");
    public static readonly RegistreerInschrijvingPersoonUitzonderingType Fout30003 = new("30003", "Onbestaand INSZ");
    public static readonly RegistreerInschrijvingPersoonUitzonderingType Fout30004 = new("30004", "Het INSZ-nummer is vervangen door een ander INSZ-nummer");

    public static readonly RegistreerInschrijvingPersoonUitzonderingType[] FoutcodesVeroorzaaktDoorGebruiker = [Fout30002, Fout30003, Fout30004];
    public static readonly string[] FoutcodesVeroorzaaktDoorGebruikerIdentificaties = FoutcodesVeroorzaaktDoorGebruiker.Select(x => x.Identificatie).ToArray();
}

public class MagdaRegistreerInschrijvingValidator
{
    private readonly ILogger _logger;

    public MagdaRegistreerInschrijvingValidator(ILogger logger)
    {
        _logger = logger;
    }

    public void ValidateOrThrow(ResponseEnvelope<RegistreerInschrijvingResponseBody>? responseEnvelope)
    {
        var antwoordUitzonderingen = responseEnvelope.Body.RegistreerInschrijvingResponse.Repliek.Antwoorden.Antwoord.Uitzonderingen;

        if(responseEnvelope.Body.RegistreerInschrijvingResponse.Repliek.Antwoorden.Antwoord.Inhoud.Resultaat.Value == ResultaatEnumType.Item0)
        {
            if (antwoordUitzonderingen is not null)
            {
                var foutcodes = antwoordUitzonderingen.Select(x => x.Identificatie).ToArray();
                LogFoutcodes(responseEnvelope, foutcodes);

                if (foutcodes.Any(x => RegistreerInschrijvingPersoonUitzonderingType.FoutcodesVeroorzaaktDoorGebruikerIdentificaties.Contains(x)))
                    throw new EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz();
            }

            throw new MagdaException();
        }
    }


    private void LogFoutcodes(ResponseEnvelope<RegistreerInschrijvingResponseBody> registreerInschrijvingResponse, IEnumerable<string> foutCodes)
    {
        _logger.LogWarning(
            "RegistreerInschrijving Persoon voor magda call reference '{Reference}' is mislukt met foutcode(s) '{FoutCodes}'",
            registreerInschrijvingResponse.Body.RegistreerInschrijvingResponse.Repliek.Context.Bericht.Ontvanger.Referte,
            string.Join(',', foutCodes));
    }
}

public class MagdaGeefPersoonService : IGeefPersoonService
{
    private readonly ILogger<MagdaGeefPersoonService> _logger;
    private readonly MagdaRegistreerInschrijvingValidator _registreerInschrijvingValidator;
    private IMagdaClient MagdaClient { get; }

    public MagdaGeefPersoonService(IMagdaClient magdaClient, ILogger<MagdaGeefPersoonService> logger)
    {
        _logger = logger;
        _registreerInschrijvingValidator = new MagdaRegistreerInschrijvingValidator(logger);
        MagdaClient = magdaClient;
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

        _registreerInschrijvingValidator.ValidateOrThrow(registreerInschrijvingResponse);

        // Second: Get person details (only runs if registration succeeded)
        var persoon = await MagdaClient.GeefPersoon(
            vertegenwoordiger.Insz,
            AanroependeFunctie.RegistreerVzer,
            metadata,
            cancellationToken);

        if(persoon.Body.GeefPersoonResponse.Repliek.Antwoorden.Antwoord.Uitzonderingen is not null)
        {
            throw new MagdaException("Er heeft zich een fout voorgedaan bij het aanroepen van de Magda GeefPersoonDienst.");
        }

        return new PersoonUitKsz(
            vertegenwoordiger.Insz,
            vertegenwoordiger.Voornaam,
            vertegenwoordiger.Achternaam,
            persoon.Body.GeefPersoonResponse.Repliek.Antwoorden.Antwoord.Inhoud.Persoon.Overlijden != null);
    }

}
