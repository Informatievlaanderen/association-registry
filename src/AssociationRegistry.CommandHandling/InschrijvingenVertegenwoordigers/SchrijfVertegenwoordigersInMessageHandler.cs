namespace AssociationRegistry.CommandHandling.InschrijvingenVertegenwoordigers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using Integrations.Magda.Shared.Exceptions;
using JasperFx.Core;
using Magda.Persoon;
using Wolverine.ErrorHandling;
using Wolverine.Runtime.Handlers;

public class SchrijfVertegenwoordigersInMessageHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IMagdaGeefPersoonService _magdaGeefPersoonService;

    public SchrijfVertegenwoordigersInMessageHandler(IVerenigingsRepository verenigingRepository, IMagdaGeefPersoonService magdaGeefPersoonService)
    {
        _verenigingRepository = verenigingRepository;
        _magdaGeefPersoonService = magdaGeefPersoonService;
    }

    public static void Configure(HandlerChain chain)
    {
        chain.OnException<MagdaException>()
             .ScheduleRetryIndefinitely(
                  10.Seconds(), 1.Minutes(), 1.Hours(), 1.Days());
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

        await vereniging.SchrijfVertegenwoordigersIn(_magdaGeefPersoonService, envelope.Metadata, cancellationToken);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
