namespace AssociationRegistry.Acties.VoegVertegenwoordigerToe;

using Framework;
using Vereniging;
using Vereniging.Exceptions;

public class VoegVertegenwoordigerToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VoegVertegenwoordigerToeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VoegVertegenwoordigerToeCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var vereniging =
                await _verenigingRepository.Load<Vereniging>(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

            vereniging.VoegVertegenwoordigerToe(envelope.Command.Vertegenwoordiger);

            var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

            return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
        }
        catch (UnsupportedOperationForVerenigingstype)
        {
            throw new VerenigingMetRechtspersoonlijkheidCannotAddVertegenwoordigers();
        }
    }
}
