namespace AssociationRegistry.Integrations.Magda.Persoon.Validation;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Models.RegistreerInschrijving0200;
using Repertorium.RegistreerInschrijving0200;
using Shared.Exceptions;
using AssociationRegistry.Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging;

public interface IMagdaRegistreerInschrijvingValidator
{
    void ValidateOrThrow(ResponseEnvelope<RegistreerInschrijvingResponseBody>? responseEnvelope);
}

public class MagdaRegistreerInschrijvingValidator : IMagdaRegistreerInschrijvingValidator
{
    private readonly ILogger<MagdaRegistreerInschrijvingValidator> _logger;

    public MagdaRegistreerInschrijvingValidator(ILogger<MagdaRegistreerInschrijvingValidator> logger)
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

                if (foutcodes.Any(x => PersoonUitzonderingType.FoutcodesVeroorzaaktDoorGebruikerIdentificaties.Contains(x)))
                    throw new EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz();

                var magdaFouten = antwoordUitzonderingen.Where(x => x.Type == UitzonderingTypeType.FOUT);

                if (magdaFouten.Any())
                {
                    throw new MagdaException();
                }
            }
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
