namespace AssociationRegistry.CommandHandling.InschrijvingenVertegenwoordigers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using Integrations.Magda.Shared.Exceptions;
using JasperFx.Core;
using Magda.Persoon;
using Microsoft.Extensions.Logging;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;

public class SchrijfVertegenwoordigersInMessageHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IMagdaGeefPersoonService _magdaGeefPersoonService;
    private readonly ILogger<SchrijfVertegenwoordigersInMessageHandler> _logger;

    public SchrijfVertegenwoordigersInMessageHandler(IVerenigingsRepository verenigingRepository, IMagdaGeefPersoonService magdaGeefPersoonService, ILogger<SchrijfVertegenwoordigersInMessageHandler> _logger)
    {
        _verenigingRepository = verenigingRepository;
        _magdaGeefPersoonService = magdaGeefPersoonService;
        this._logger = _logger;
    }

    public static void Configure(HandlerChain chain)
    {
        chain.OnException<MagdaException>()
             .ScheduleRetry(
                  10.Seconds(), 1.Minutes(), 1.Hours(), 1.Days())
             .Then
             .MoveToErrorQueue();

        chain.OnException<TaskCanceledException>()
             .ScheduleRetry(
                  10.Seconds(), 1.Minutes(), 1.Hours(), 1.Days())
             .Then
             .MoveToErrorQueue();
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<SchrijfVertegenwoordigersInMessage> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<Vereniging>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata,
                allowDubbeleVereniging: true,
                allowVerwijderdeVereniging: true);

        await vereniging.SchrijfVertegenwoordigersIn(_magdaGeefPersoonService, envelope.Metadata, cancellationToken, _logger);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        _logger.LogInformation($"SchrijfVertegenwoordigersInMessageHandler successful for VCode {envelope.Command.VCode}");

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
