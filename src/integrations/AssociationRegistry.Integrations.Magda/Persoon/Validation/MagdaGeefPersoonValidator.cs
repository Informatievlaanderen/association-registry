namespace AssociationRegistry.Integrations.Magda.Persoon.Validation;

using DecentraalBeheer.Vereniging.Exceptions;
using Microsoft.Extensions.Logging;
using Models;
using Shared.Exceptions;
using Shared.Models;
using UitzonderingType = GeefPersoon.UitzonderingType;
using UitzonderingTypeType = GeefPersoon.UitzonderingTypeType;

public interface IMagdaGeefPersoonValidator
{
    void ValidateOrThrow(ResponseEnvelope<GeefPersoonResponseBody>? responseEnvelope, Guid correlationId);
}

public class MagdaGeefPersoonValidator : IMagdaGeefPersoonValidator
{
    private readonly ILogger<MagdaGeefPersoonValidator> _logger;

    public MagdaGeefPersoonValidator(ILogger<MagdaGeefPersoonValidator> logger)
    {
        _logger = logger;
    }

    public void ValidateOrThrow(ResponseEnvelope<GeefPersoonResponseBody>? responseEnvelope, Guid correlationId)
    {
        var fault = responseEnvelope.Body.Fault;

        if (fault != null)
            throw new MagdaException();


        var antwoordUitzonderingen = responseEnvelope.Body.GeefPersoonResponse.Repliek.Antwoorden.Antwoord.Uitzonderingen;

        if (antwoordUitzonderingen is not null)
        {
            var foutcodes = antwoordUitzonderingen.Select(x => x.Identificatie).ToArray();
            var fouten = antwoordUitzonderingen
               .Select(x => (Foutcode: x.Identificatie, Diagnose: x.Diagnose));
            LogFoutcodes(responseEnvelope, fouten, correlationId);

            var gebruikersUitzonderingen = antwoordUitzonderingen.Where(IsGekendeGebruikersFout).ToArray();
            if (gebruikersUitzonderingen.Any())
                throw new EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz();

            var magdaFouten = antwoordUitzonderingen.Where(x => x.Type == UitzonderingTypeType.FOUT);

            if (magdaFouten.Any())
            {
                throw new MagdaException();
            }
        }
    }

    private static bool IsGekendeGebruikersFout(UitzonderingType x)
        => PersoonUitzonderingType.FoutcodesVeroorzaaktDoorGebruikerIdentificaties.Contains(x.Identificatie);

    private void LogFoutcodes(
        ResponseEnvelope<GeefPersoonResponseBody> GeefPersoonResponse,
        IEnumerable<(string Foutcode, string Diagnose)> fouten,
        Guid correlationId)
    {
        _logger.LogWarning(
            "GeefPersoon voor magda call reference '{Reference}' is mislukt met foutcode(s) '{FoutCodes}'. Met ID: {CorrelationId}",
            GeefPersoonResponse.Body.GeefPersoonResponse.Repliek.Context.Bericht.Ontvanger.Referte,
            string.Join(", ", fouten.Select(f => $"{f.Foutcode}:{f.Diagnose}")),
            correlationId);
    }
}
