namespace AssociationRegistry.Integrations.Magda.Persoon.Validation;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Models.RegistreerInschrijving0200;
using Repertorium.RegistreerInschrijving0200;
using Shared.Exceptions;
using AssociationRegistry.Integrations.Magda.Shared.Models;
using Microsoft.Extensions.Logging;

public interface IMagdaRegistreerInschrijvingValidator
{
    void ValidateOrThrow(ResponseEnvelope<RegistreerInschrijvingResponseBody>? responseEnvelope, Guid correlationId);
}

public class MagdaRegistreerInschrijvingValidator : IMagdaRegistreerInschrijvingValidator
{
    private readonly ILogger<MagdaRegistreerInschrijvingValidator> _logger;

    public MagdaRegistreerInschrijvingValidator(ILogger<MagdaRegistreerInschrijvingValidator> logger)
    {
        _logger = logger;
    }

    public void ValidateOrThrow(ResponseEnvelope<RegistreerInschrijvingResponseBody>? responseEnvelope, Guid correlationId)
    {
        var fault = responseEnvelope.Body.Fault;

        if (fault != null)
            throw new MagdaException();

        _logger.LogInformation($"Registreer Inschrijving: Referte {responseEnvelope.Body.RegistreerInschrijvingResponse.Repliek.Context.Bericht.Ontvanger.Referte} ID: {correlationId}");

        if (responseEnvelope.Body.RegistreerInschrijvingResponse.Repliek.Antwoorden.Antwoord.Inhoud.Resultaat.Value ==
            ResultaatEnumType.Item1)
        {
            _logger.LogInformation($"Registreer Inschrijving: {responseEnvelope.Body.RegistreerInschrijvingResponse.Repliek.Antwoorden.Antwoord.Inhoud.Resultaat.Beschrijving} met ID {correlationId}");
            return;
        }

        var antwoordUitzonderingen = responseEnvelope.Body.RegistreerInschrijvingResponse.Repliek.Antwoorden.Antwoord.Uitzonderingen;
        if (antwoordUitzonderingen is not null)
        {
            var foutcodes = antwoordUitzonderingen.Select(x => x.Identificatie).ToArray();
            var fouten = antwoordUitzonderingen
               .Select(x => (Foutcode: x.Identificatie, Diagnose: x.Diagnose));
            LogFoutcodes(responseEnvelope, fouten, correlationId);
            if (foutcodes.Any(x => PersoonUitzonderingType.FoutcodesVeroorzaaktDoorGebruikerIdentificaties.Contains(x)))
                throw new EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz();

            var magdaFouten = antwoordUitzonderingen.Where(x => x.Type == UitzonderingTypeType.FOUT);

            if(!magdaFouten.Any())
                return;
        }

        throw new MagdaException();
    }


    private void LogFoutcodes(
        ResponseEnvelope<RegistreerInschrijvingResponseBody> registreerInschrijvingResponse,
        IEnumerable<(string Foutcode, string Diagnose)> fouten,
        Guid correlationId)
    {
        _logger.LogWarning(
            "RegistreerInschrijving Persoon voor magda call reference '{Reference}' is mislukt met fout(en) '{Fouten}'. \n Met ID: {CorrelationId}",
            registreerInschrijvingResponse.Body.RegistreerInschrijvingResponse.Repliek.Context.Bericht.Ontvanger.Referte,
            string.Join(", ", fouten.Select(f => $"{f.Foutcode}:{f.Diagnose}")),
            correlationId);
    }
}
