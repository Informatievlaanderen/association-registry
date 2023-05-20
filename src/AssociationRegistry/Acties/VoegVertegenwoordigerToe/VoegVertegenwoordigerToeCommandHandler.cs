namespace AssociationRegistry.Acties.VoegVertegenwoordigerToe;

using Framework;
using Magda;
using Vereniging;

public class VoegVertegenwoordigerToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IMagdaFacade _magdaFacade;

    public VoegVertegenwoordigerToeCommandHandler(IVerenigingsRepository verenigingRepository, IMagdaFacade magdaFacade)
    {
        _verenigingRepository = verenigingRepository;
        _magdaFacade = magdaFacade;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VoegVertegenwoordigerToeCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingRepository.Load(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);
        var vertegenwoordigerService = new VertegenwoordigerService(_magdaFacade);
        var vertegenwoordiger = await vertegenwoordigerService.GetVertegenwoordiger(envelope.Command.Vertegenwoordiger);

        vereniging.VoegVertegenwoordigerToe(vertegenwoordiger);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);
        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
