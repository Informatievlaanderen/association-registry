namespace AssociationRegistry.Admin.Api.Magda;

using System;
using System.Threading;
using System.Threading.Tasks;
using Kbo;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Exceptions;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Onderneming.GeefOndernemingVKBO;
using Microsoft.Extensions.Logging;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    private readonly IMagdaCallReferenceRepository _repository;
    private readonly IMagdaFacade _magdaFacade;
    private readonly ILogger<MagdaGeefVerenigingService> _logger;

    public MagdaGeefVerenigingService(
        IMagdaCallReferenceRepository repository,
        IMagdaFacade magdaFacade,
        ILogger<MagdaGeefVerenigingService> logger)
    {
        _repository = repository;
        _magdaFacade = magdaFacade;
        _logger = logger;
    }

    public async Task<Result> GeefVereniging(KboNummer kboNummer, string initiator,CancellationToken cancellationToken)
    {
        try
        {
            var reference = await CreateReference(_repository, initiator, cancellationToken);
            var magdaResponse = await _magdaFacade.GeefOnderneming(kboNummer, reference);

            if (HasFoutUitzonderingen(magdaResponse))
                return HandleUitzonderingen(kboNummer, magdaResponse);

            return VerenigingVolgensKboResult.GeldigeVereniging(
                new VerenigingVolgensKbo
                {
                    KboNummer = KboNummer.Create(kboNummer),
                });
        }
        catch(Exception e)
        {
            throw new MagdaException("Something went Wrong with magda. Look at inner exception for moer details",e);
        }
    }

    private Result HandleUitzonderingen(string kboNummer, Envelope<GeefOndernemingResponseBody>? magdaResponse)
    {
        _logger.LogInformation(
            "Uitzondering bij het aanroepen van de Magda GeefOndernemingVKBO service voor KBO-nummer {KboNummer}: '{Diagnose}'",
            kboNummer,
            magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Uitzonderingen.ConcatenateUitzonderingen("\n"));

        return VerenigingVolgensKboResult.GeenGeldigeVereniging;
    }

    private static bool HasFoutUitzonderingen(Envelope<GeefOndernemingResponseBody>? magdaResponse)
        => magdaResponse.HasUitzonderingenOfTypes(UitzonderingTypeType.FOUT);

    private static async Task<MagdaCallReference> CreateReference(IMagdaCallReferenceRepository repository, string initiator, CancellationToken cancellationToken)
    {
        var magdaCallReference = new MagdaCallReference
        {
            Reference = Guid.NewGuid(),
            CalledAt = DateTimeOffset.UtcNow,
            Initiator = initiator,
        };
        await repository.Save(magdaCallReference, cancellationToken);

        return magdaCallReference;
    }
}
