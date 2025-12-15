namespace AssociationRegistry.CommandHandling.MagdaSync.SyncKsz;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using KboSyncLambda.SyncKsz;
using Microsoft.Extensions.Logging;

public class SyncVertegenwoordigerCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IMagdaGeefPersoonService _magdaGeefPersoonService;
    private readonly ILogger<SyncVertegenwoordigerCommandHandler> _logger;

    public SyncVertegenwoordigerCommandHandler(IVerenigingsRepository verenigingRepository, IMagdaGeefPersoonService magdaGeefPersoonService, ILogger<SyncVertegenwoordigerCommandHandler> _logger)
    {
        _verenigingRepository = verenigingRepository;
        _magdaGeefPersoonService = magdaGeefPersoonService;
        this._logger = _logger;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<SyncVertegenwoordigerCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<Vereniging>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata,
                allowDubbeleVereniging: true,
                allowVerwijderdeVereniging: true);

        await vereniging.SyncVertegenwoordiger(_magdaGeefPersoonService, envelope.Command.VertegenwoordigerId, envelope.Metadata, cancellationToken);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        _logger.LogInformation($"SyncVertegenwoordigerCommandHandler successful for VCode {envelope.Command.VCode}");

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
