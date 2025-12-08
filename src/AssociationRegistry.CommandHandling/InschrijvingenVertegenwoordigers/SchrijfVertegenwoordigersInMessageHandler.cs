namespace AssociationRegistry.CommandHandling.InschrijvingenVertegenwoordigers;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using Magda.Persoon;

public class SchrijfVertegenwoordigersInMessageHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IMagdaGeefPersoonService _magdaGeefPersoonService;

    public SchrijfVertegenwoordigersInMessageHandler(IVerenigingsRepository verenigingRepository, IMagdaGeefPersoonService magdaGeefPersoonService)
    {
        _verenigingRepository = verenigingRepository;
        _magdaGeefPersoonService = magdaGeefPersoonService;
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
